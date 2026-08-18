using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using System.Globalization;
using System.Text;

namespace SchulApp.Services
{
    public enum ExportDataSet
    {
        Schueler,
        Klassen,
        Stundenplan,
        AuditLog
    }

    public enum ExportFileFormat
    {
        Csv,
        Pdf
    }

    public sealed class CsvImportResult
    {
        public int ImportedCount { get; init; }

        public int FailedCount { get; init; }

        public IReadOnlyList<string> Errors { get; init; }
            = Array.Empty<string>();
    }

    public sealed class StudentCsvValidationResult
    {
        public IReadOnlyList<SchuelerModel> Students { get; init; }
            = Array.Empty<SchuelerModel>();

        public IReadOnlyList<string> Errors { get; init; }
            = Array.Empty<string>();
    }

    public static class DataTransferService
    {
        private const char CsvDelimiter = ';';
        private const int PdfMaxLineLength = 95;
        private const int PdfLinesPerPage = 48;

        public static async Task ExportAsync(
            ExportDataSet dataSet,
            ExportFileFormat format,
            string filePath)
        {
            await using SchulAppContext db = new SchulAppContext();

            ExportDocument document =
                await BuildExportDocumentAsync(db, dataSet);

            if (format == ExportFileFormat.Csv)
            {
                string csv = CreateCsv(
                    document.Headers,
                    document.Rows
                );

                await File.WriteAllTextAsync(
                    filePath,
                    csv,
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)
                );

                return;
            }

            byte[] pdf = CreatePdf(
                document.Title,
                document.Headers,
                document.Rows
            );

            await File.WriteAllBytesAsync(filePath, pdf);
        }

        public static async Task<CsvImportResult> ImportStudentsAsync(
            string filePath)
        {
            string csv = await File.ReadAllTextAsync(
                filePath,
                Encoding.UTF8
            );

            await using SchulAppContext db = new SchulAppContext();

            List<int> validClassIds = await db.Klassen
                .AsNoTracking()
                .Select(x => x.KlassenId)
                .ToListAsync();

            List<SchuelerModel> existingStudents = await db.Schueler
                .AsNoTracking()
                .Select(x => new SchuelerModel
                {
                    SchuelerId = x.SchuelerId,
                    Name = x.Name,
                    KlasseId = x.KlasseId
                })
                .ToListAsync();

            StudentCsvValidationResult validation =
                ValidateStudentCsv(
                    csv,
                    validClassIds,
                    existingStudents
                );

            if (validation.Errors.Count > 0)
            {
                return new CsvImportResult
                {
                    ImportedCount = 0,
                    FailedCount = validation.Errors.Count,
                    Errors = validation.Errors
                };
            }

            db.Schueler.AddRange(validation.Students);
            await db.SaveChangesAsync();

            return new CsvImportResult
            {
                ImportedCount = validation.Students.Count,
                FailedCount = 0,
                Errors = Array.Empty<string>()
            };
        }

        public static StudentCsvValidationResult ValidateStudentCsv(
            string csv,
            IEnumerable<int> validClassIds,
            IEnumerable<SchuelerModel>? existingStudents = null)
        {
            List<string> errors = new List<string>();
            CsvTable table;

            try
            {
                table = ParseCsv(csv);
            }
            catch (FormatException exception)
            {
                return new StudentCsvValidationResult
                {
                    Errors = new[]
                    {
                        $"CSV konnte nicht gelesen werden: {exception.Message}"
                    }
                };
            }

            int nameIndex = FindHeaderIndex(table.Headers, "Name");
            int classIdIndex = FindHeaderIndex(
                table.Headers,
                "KlasseId"
            );

            if (nameIndex < 0 || classIdIndex < 0)
            {
                return new StudentCsvValidationResult
                {
                    Errors = new[]
                    {
                        "Die CSV-Datei muss die Spalten 'Name' und 'KlasseId' enthalten."
                    }
                };
            }

            HashSet<int> knownClassIds = new HashSet<int>(validClassIds);
            HashSet<string> knownStudents = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            );

            foreach (SchuelerModel student in
                existingStudents ?? Enumerable.Empty<SchuelerModel>())
            {
                knownStudents.Add(
                    CreateStudentKey(student.Name, student.KlasseId)
                );
            }

            List<SchuelerModel> students = new List<SchuelerModel>();

            foreach (CsvRow row in table.Rows)
            {
                string name = GetCell(row, nameIndex).Trim();
                string classIdText = GetCell(row, classIdIndex).Trim();

                if (string.IsNullOrWhiteSpace(name))
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Name fehlt."
                    );
                    continue;
                }

                int? classId = null;

                if (!string.IsNullOrWhiteSpace(classIdText))
                {
                    if (!int.TryParse(
                        classIdText,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int parsedClassId))
                    {
                        errors.Add(
                            $"Zeile {row.LineNumber}: KlasseId '{classIdText}' ist keine gültige Zahl."
                        );
                        continue;
                    }

                    if (!knownClassIds.Contains(parsedClassId))
                    {
                        errors.Add(
                            $"Zeile {row.LineNumber}: KlasseId {parsedClassId} existiert nicht."
                        );
                        continue;
                    }

                    classId = parsedClassId;
                }

                string studentKey = CreateStudentKey(name, classId);

                if (!knownStudents.Add(studentKey))
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Schüler '{name}' ist für diese Klasse bereits vorhanden."
                    );
                    continue;
                }

                students.Add(new SchuelerModel
                {
                    Name = name,
                    KlasseId = classId
                });
            }

            return new StudentCsvValidationResult
            {
                Students = students,
                Errors = errors
            };
        }

        public static string CreateCsv(
            IReadOnlyList<string> headers,
            IEnumerable<IReadOnlyList<string>> rows)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine(
                string.Join(
                    CsvDelimiter,
                    headers.Select(EscapeCsvValue)
                )
            );

            foreach (IReadOnlyList<string> row in rows)
            {
                builder.AppendLine(
                    string.Join(
                        CsvDelimiter,
                        row.Select(EscapeCsvValue)
                    )
                );
            }

            return builder.ToString();
        }

        public static byte[] CreatePdf(
            string title,
            IReadOnlyList<string> headers,
            IEnumerable<IReadOnlyList<string>> rows)
        {
            List<string> lines = new List<string>
            {
                title,
                string.Empty,
                string.Join(" | ", headers),
                new string('-', Math.Min(PdfMaxLineLength, 90))
            };

            foreach (IReadOnlyList<string> row in rows)
            {
                string line = string.Join(" | ", row);
                lines.AddRange(WrapPdfLine(line));
            }

            if (lines.Count == 4)
            {
                lines.Add("Keine Daten vorhanden.");
            }

            return BuildSimplePdf(lines);
        }

        private static async Task<ExportDocument> BuildExportDocumentAsync(
            SchulAppContext db,
            ExportDataSet dataSet)
        {
            switch (dataSet)
            {
                case ExportDataSet.Schueler:
                {
                    List<SchuelerModel> students = await db.Schueler
                        .AsNoTracking()
                        .Include(x => x.Klasse)
                        .OrderBy(x => x.Name)
                        .ThenBy(x => x.SchuelerId)
                        .ToListAsync();

                    List<IReadOnlyList<string>> rows = students
                        .Select(x => (IReadOnlyList<string>)new[]
                        {
                            x.SchuelerId.ToString(
                                CultureInfo.InvariantCulture
                            ),
                            x.Name,
                            x.KlasseId?.ToString(
                                CultureInfo.InvariantCulture
                            ) ?? string.Empty,
                            x.Klasse?.Bezeichnung ?? string.Empty
                        })
                        .ToList();

                    return new ExportDocument(
                        "Schülerliste",
                        new[] { "ID", "Name", "Klasse-ID", "Klasse" },
                        rows
                    );
                }

                case ExportDataSet.Klassen:
                {
                    List<KlasseModel> classes = await db.Klassen
                        .AsNoTracking()
                        .OrderBy(x => x.Bezeichnung)
                        .ToListAsync();

                    List<IReadOnlyList<string>> rows = classes
                        .Select(x => (IReadOnlyList<string>)new[]
                        {
                            x.KlassenId.ToString(
                                CultureInfo.InvariantCulture
                            ),
                            x.Bezeichnung,
                            x.KlassenlehrerId?.ToString(
                                CultureInfo.InvariantCulture
                            ) ?? string.Empty
                        })
                        .ToList();

                    return new ExportDocument(
                        "Klassenliste",
                        new[]
                        {
                            "Klassen-ID",
                            "Bezeichnung",
                            "Klassenlehrer-ID"
                        },
                        rows
                    );
                }

                case ExportDataSet.Stundenplan:
                {
                    List<StundenplanModel> timetable = await db.Stundenplan
                        .AsNoTracking()
                        .Include(x => x.Kurs)
                        .ThenInclude(x => x.Klasse)
                        .OrderBy(x => x.Wochentag)
                        .ThenBy(x => x.Startzeit)
                        .ToListAsync();

                    List<IReadOnlyList<string>> rows = timetable
                        .Select(x => (IReadOnlyList<string>)new[]
                        {
                            x.StundenplanId.ToString(
                                CultureInfo.InvariantCulture
                            ),
                            WeekdayName(x.Wochentag),
                            x.Startzeit.ToString(@"hh\:mm"),
                            x.Endzeit.ToString(@"hh\:mm"),
                            x.Kurs?.Name ?? string.Empty,
                            x.Kurs?.Klasse?.Bezeichnung ?? string.Empty,
                            x.Raum ?? string.Empty
                        })
                        .ToList();

                    return new ExportDocument(
                        "Stundenplan",
                        new[]
                        {
                            "ID",
                            "Wochentag",
                            "Start",
                            "Ende",
                            "Kurs",
                            "Klasse",
                            "Raum"
                        },
                        rows
                    );
                }

                case ExportDataSet.AuditLog:
                {
                    List<AuditLogModel> logs = await db.AuditLogs
                        .AsNoTracking()
                        .OrderByDescending(x => x.TimestampUtc)
                        .ToListAsync();

                    List<IReadOnlyList<string>> rows = logs
                        .Select(x => (IReadOnlyList<string>)new[]
                        {
                            x.Id.ToString(CultureInfo.InvariantCulture),
                            x.TimestampUtc.ToString(
                                "yyyy-MM-dd HH:mm:ss",
                                CultureInfo.InvariantCulture
                            ),
                            x.UserName,
                            x.UserRole ?? string.Empty,
                            x.Action,
                            x.EntityType,
                            x.EntityId ?? string.Empty,
                            x.Changes ?? string.Empty,
                            x.Source
                        })
                        .ToList();

                    return new ExportDocument(
                        "Audit Log",
                        new[]
                        {
                            "ID",
                            "Timestamp UTC",
                            "Benutzer",
                            "Rolle",
                            "Aktion",
                            "Entität",
                            "Entitäts-ID",
                            "Änderungen",
                            "Quelle"
                        },
                        rows
                    );
                }

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(dataSet),
                        dataSet,
                        "Unbekannter Export-Datensatz."
                    );
            }
        }

        private static CsvTable ParseCsv(string csv)
        {
            using StringReader reader = new StringReader(csv ?? string.Empty);

            string? headerLine = null;
            int lineNumber = 0;

            while (headerLine == null)
            {
                string? currentLine = reader.ReadLine();
                lineNumber++;

                if (currentLine == null)
                {
                    throw new FormatException(
                        "Die Datei enthält keine Kopfzeile."
                    );
                }

                if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    headerLine = currentLine;
                }
            }

            char delimiter = DetectDelimiter(headerLine);
            List<string> headers = ParseCsvLine(
                headerLine,
                delimiter
            );

            if (headers.Count > 0)
            {
                headers[0] = headers[0].TrimStart('\uFEFF');
            }

            List<CsvRow> rows = new List<CsvRow>();
            string? line;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                rows.Add(new CsvRow
                {
                    LineNumber = lineNumber,
                    Values = ParseCsvLine(line, delimiter)
                });
            }

            return new CsvTable
            {
                Headers = headers,
                Rows = rows
            };
        }

        private static char DetectDelimiter(string headerLine)
        {
            int semicolonCount = CountDelimiter(headerLine, ';');
            int commaCount = CountDelimiter(headerLine, ',');

            return semicolonCount >= commaCount ? ';' : ',';
        }

        private static int CountDelimiter(string line, char delimiter)
        {
            int count = 0;
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char current = line[i];

                if (current == '"')
                {
                    if (inQuotes &&
                        i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        i++;
                        continue;
                    }

                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && current == delimiter)
                {
                    count++;
                }
            }

            return count;
        }

        private static List<string> ParseCsvLine(
            string line,
            char delimiter)
        {
            List<string> values = new List<string>();
            StringBuilder field = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char current = line[i];

                if (current == '"')
                {
                    if (inQuotes &&
                        i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                        continue;
                    }

                    inQuotes = !inQuotes;
                    continue;
                }

                if (!inQuotes && current == delimiter)
                {
                    values.Add(field.ToString());
                    field.Clear();
                    continue;
                }

                field.Append(current);
            }

            if (inQuotes)
            {
                throw new FormatException(
                    "Eine CSV-Zeile enthält ein nicht geschlossenes Anführungszeichen."
                );
            }

            values.Add(field.ToString());
            return values;
        }

        private static int FindHeaderIndex(
            IReadOnlyList<string> headers,
            string name)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                if (string.Equals(
                    headers[i].Trim(),
                    name,
                    StringComparison.OrdinalIgnoreCase
                ))
                {
                    return i;
                }
            }

            return -1;
        }

        private static string GetCell(CsvRow row, int index)
        {
            return index >= 0 && index < row.Values.Count
                ? row.Values[index]
                : string.Empty;
        }

        private static string CreateStudentKey(
            string name,
            int? classId)
        {
            return $"{name.Trim()}|{classId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty}";
        }

        private static string EscapeCsvValue(string? value)
        {
            string text = value ?? string.Empty;

            if (!text.Contains(CsvDelimiter) &&
                !text.Contains('"') &&
                !text.Contains('\r') &&
                !text.Contains('\n'))
            {
                return text;
            }

            return $"\"{text.Replace("\"", "\"\"")}\"";
        }

        private static string WeekdayName(byte weekday)
        {
            return weekday switch
            {
                1 => "Montag",
                2 => "Dienstag",
                3 => "Mittwoch",
                4 => "Donnerstag",
                5 => "Freitag",
                6 => "Samstag",
                7 => "Sonntag",
                _ => weekday.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static IEnumerable<string> WrapPdfLine(string line)
        {
            string remaining = line;

            if (remaining.Length == 0)
            {
                yield return string.Empty;
                yield break;
            }

            while (remaining.Length > PdfMaxLineLength)
            {
                int splitIndex = remaining.LastIndexOf(
                    ' ',
                    PdfMaxLineLength
                );

                if (splitIndex < PdfMaxLineLength / 2)
                {
                    splitIndex = PdfMaxLineLength;
                }

                yield return remaining[..splitIndex].TrimEnd();
                remaining = remaining[splitIndex..].TrimStart();
            }

            yield return remaining;
        }

        private static byte[] BuildSimplePdf(
            IReadOnlyList<string> sourceLines)
        {
            List<List<string>> pages = new List<List<string>>();

            for (int i = 0; i < sourceLines.Count; i += PdfLinesPerPage)
            {
                pages.Add(
                    sourceLines
                        .Skip(i)
                        .Take(PdfLinesPerPage)
                        .ToList()
                );
            }

            if (pages.Count == 0)
            {
                pages.Add(new List<string> { "Keine Daten vorhanden." });
            }

            int objectCount = 3 + pages.Count * 2;
            long[] offsets = new long[objectCount + 1];

            using MemoryStream stream = new MemoryStream();

            void Write(string text)
            {
                byte[] bytes = Encoding.Latin1.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
            }

            void WriteObject(int id, string body)
            {
                offsets[id] = stream.Position;
                Write($"{id} 0 obj\n{body}\nendobj\n");
            }

            Write("%PDF-1.4\n%âãÏÓ\n");

            WriteObject(
                1,
                "<< /Type /Catalog /Pages 2 0 R >>"
            );

            string pageReferences = string.Join(
                " ",
                Enumerable.Range(0, pages.Count)
                    .Select(i => $"{4 + i * 2} 0 R")
            );

            WriteObject(
                2,
                $"<< /Type /Pages /Kids [{pageReferences}] /Count {pages.Count} >>"
            );

            WriteObject(
                3,
                "<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica /Encoding /WinAnsiEncoding >>"
            );

            for (int pageIndex = 0; pageIndex < pages.Count; pageIndex++)
            {
                int pageObjectId = 4 + pageIndex * 2;
                int contentObjectId = pageObjectId + 1;

                WriteObject(
                    pageObjectId,
                    $"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Resources << /Font << /F1 3 0 R >> >> /Contents {contentObjectId} 0 R >>"
                );

                StringBuilder content = new StringBuilder();
                int y = 805;

                for (int lineIndex = 0;
                    lineIndex < pages[pageIndex].Count;
                    lineIndex++)
                {
                    string line = pages[pageIndex][lineIndex];
                    int fontSize = pageIndex == 0 && lineIndex == 0
                        ? 14
                        : 10;

                    content.Append("BT /F1 ")
                        .Append(fontSize)
                        .Append(" Tf 1 0 0 1 40 ")
                        .Append(y)
                        .Append(" Tm (")
                        .Append(EscapePdfText(line))
                        .Append(") Tj ET\n");

                    y -= lineIndex == 0 && pageIndex == 0
                        ? 22
                        : 15;
                }

                string contentText = content.ToString();
                int contentLength = Encoding.Latin1.GetByteCount(
                    contentText
                );

                WriteObject(
                    contentObjectId,
                    $"<< /Length {contentLength} >>\nstream\n{contentText}endstream"
                );
            }

            long xrefOffset = stream.Position;

            Write($"xref\n0 {objectCount + 1}\n");
            Write("0000000000 65535 f \n");

            for (int id = 1; id <= objectCount; id++)
            {
                Write($"{offsets[id]:0000000000} 00000 n \n");
            }

            Write(
                $"trailer\n<< /Size {objectCount + 1} /Root 1 0 R >>\nstartxref\n{xrefOffset}\n%%EOF\n"
            );

            return stream.ToArray();
        }

        private static string EscapePdfText(string value)
        {
            StringBuilder builder = new StringBuilder();

            foreach (char current in value)
            {
                char safeCharacter = current <= byte.MaxValue
                    ? current
                    : '?';

                if (safeCharacter == '\\' ||
                    safeCharacter == '(' ||
                    safeCharacter == ')')
                {
                    builder.Append('\\');
                }

                builder.Append(safeCharacter);
            }

            return builder.ToString();
        }

        private sealed class ExportDocument
        {
            public ExportDocument(
                string title,
                IReadOnlyList<string> headers,
                IReadOnlyList<IReadOnlyList<string>> rows)
            {
                Title = title;
                Headers = headers;
                Rows = rows;
            }

            public string Title { get; }

            public IReadOnlyList<string> Headers { get; }

            public IReadOnlyList<IReadOnlyList<string>> Rows { get; }
        }

        private sealed class CsvTable
        {
            public IReadOnlyList<string> Headers { get; init; }
                = Array.Empty<string>();

            public IReadOnlyList<CsvRow> Rows { get; init; }
                = Array.Empty<CsvRow>();
        }

        private sealed class CsvRow
        {
            public int LineNumber { get; init; }

            public IReadOnlyList<string> Values { get; init; }
                = Array.Empty<string>();
        }
    }
}
