using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using System.Globalization;
using System.Text;

namespace SchulApp.Services
{
    public sealed class TimetableCsvValidationResult
    {
        public IReadOnlyList<StundenplanModel> Entries { get; init; }
            = Array.Empty<StundenplanModel>();

        public IReadOnlyList<string> Errors { get; init; }
            = Array.Empty<string>();
    }

    public static class TimetableDataTransferService
    {
        public static async Task ExportAsync(
            ExportFileFormat format,
            string filePath,
            int? klasseId = null)
        {
            await using SchulAppContext db = new SchulAppContext();

            IQueryable<StundenplanModel> query = db.Stundenplan
                .AsNoTracking()
                .Include(x => x.Kurs)
                .ThenInclude(x => x.Klasse);

            if (klasseId.HasValue)
            {
                query = query.Where(
                    x => x.Kurs.KlasseId == klasseId.Value
                );
            }

            List<StundenplanModel> entries = await query
                .OrderBy(x => x.Wochentag)
                .ThenBy(x => x.Startzeit)
                .ThenBy(x => x.KursId)
                .ToListAsync();

            IReadOnlyList<string> headers = new[]
            {
                "ID",
                "Kurs-ID",
                "Wochentag",
                "Start",
                "Ende",
                "Kurs",
                "Klasse",
                "Raum"
            };

            List<IReadOnlyList<string>> rows = entries
                .Select(x => (IReadOnlyList<string>)new[]
                {
                    x.StundenplanId.ToString(CultureInfo.InvariantCulture),
                    x.KursId.ToString(CultureInfo.InvariantCulture),
                    WochentagName(x.Wochentag),
                    x.Startzeit.ToString(@"hh\:mm"),
                    x.Endzeit.ToString(@"hh\:mm"),
                    x.Kurs.Name,
                    x.Kurs.Klasse.Bezeichnung,
                    x.Raum ?? string.Empty
                })
                .ToList();

            if (format == ExportFileFormat.Csv)
            {
                await File.WriteAllTextAsync(
                    filePath,
                    DataTransferService.CreateCsv(headers, rows),
                    new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)
                );
                return;
            }

            await File.WriteAllBytesAsync(
                filePath,
                DataTransferService.CreatePdf(
                    klasseId.HasValue ? "Mein Stundenplan" : "Stundenplan",
                    headers,
                    rows
                )
            );
        }

        public static async Task<CsvImportResult> ImportAsync(string filePath)
        {
            string csv = await File.ReadAllTextAsync(filePath, Encoding.UTF8);

            await using SchulAppContext db = new SchulAppContext();

            List<int> validCourseIds = await db.Kurse
                .AsNoTracking()
                .Select(x => x.KursId)
                .ToListAsync();

            List<StundenplanModel> existingEntries = await db.Stundenplan
                .AsNoTracking()
                .Select(x => new StundenplanModel
                {
                    KursId = x.KursId,
                    Wochentag = x.Wochentag,
                    Startzeit = x.Startzeit,
                    Endzeit = x.Endzeit,
                    Raum = x.Raum
                })
                .ToListAsync();

            TimetableCsvValidationResult validation = ValidateCsv(
                csv,
                validCourseIds,
                existingEntries
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

            db.Stundenplan.AddRange(validation.Entries);
            await db.SaveChangesAsync();

            return new CsvImportResult
            {
                ImportedCount = validation.Entries.Count,
                FailedCount = 0,
                Errors = Array.Empty<string>()
            };
        }

        public static TimetableCsvValidationResult ValidateCsv(
            string csv,
            IEnumerable<int> validCourseIds,
            IEnumerable<StundenplanModel>? existingEntries = null)
        {
            CsvTable table;

            try
            {
                table = ParseCsv(csv);
            }
            catch (FormatException ex)
            {
                return new TimetableCsvValidationResult
                {
                    Errors = new[]
                    {
                        $"CSV konnte nicht gelesen werden: {ex.Message}"
                    }
                };
            }

            int courseIndex = FindHeaderIndex(
                table.Headers,
                "Kurs-ID",
                "KursId"
            );
            int weekdayIndex = FindHeaderIndex(table.Headers, "Wochentag");
            int startIndex = FindHeaderIndex(table.Headers, "Start");
            int endIndex = FindHeaderIndex(table.Headers, "Ende");
            int roomIndex = FindHeaderIndex(table.Headers, "Raum");

            if (courseIndex < 0 ||
                weekdayIndex < 0 ||
                startIndex < 0 ||
                endIndex < 0)
            {
                return new TimetableCsvValidationResult
                {
                    Errors = new[]
                    {
                        "Die CSV-Datei muss die Spalten 'Kurs-ID', 'Wochentag', 'Start' und 'Ende' enthalten."
                    }
                };
            }

            HashSet<int> knownCourseIds = new HashSet<int>(validCourseIds);
            HashSet<string> knownEntries = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase
            );

            foreach (StundenplanModel entry in
                     existingEntries ?? Enumerable.Empty<StundenplanModel>())
            {
                knownEntries.Add(CreateEntryKey(entry));
            }

            List<StundenplanModel> entries = new List<StundenplanModel>();
            List<string> errors = new List<string>();

            foreach (CsvRow row in table.Rows)
            {
                string courseText = GetCell(row, courseIndex).Trim();
                string weekdayText = GetCell(row, weekdayIndex).Trim();
                string startText = GetCell(row, startIndex).Trim();
                string endText = GetCell(row, endIndex).Trim();
                string room = roomIndex >= 0
                    ? GetCell(row, roomIndex).Trim()
                    : string.Empty;

                if (!int.TryParse(
                        courseText,
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int courseId) ||
                    !knownCourseIds.Contains(courseId))
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Kurs-ID '{courseText}' existiert nicht."
                    );
                    continue;
                }

                if (!TryParseWochentag(weekdayText, out byte weekday))
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Wochentag '{weekdayText}' ist ungültig."
                    );
                    continue;
                }

                if (!TryParseTime(startText, out TimeSpan start))
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Startzeit '{startText}' ist ungültig."
                    );
                    continue;
                }

                if (!TryParseTime(endText, out TimeSpan end))
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Endzeit '{endText}' ist ungültig."
                    );
                    continue;
                }

                if (end <= start)
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Die Endzeit muss nach der Startzeit liegen."
                    );
                    continue;
                }

                if (room.Length > 50)
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Der Raum darf höchstens 50 Zeichen lang sein."
                    );
                    continue;
                }

                StundenplanModel entry = new StundenplanModel
                {
                    KursId = courseId,
                    Wochentag = weekday,
                    Startzeit = start,
                    Endzeit = end,
                    Raum = string.IsNullOrWhiteSpace(room) ? null : room
                };

                if (!knownEntries.Add(CreateEntryKey(entry)))
                {
                    errors.Add(
                        $"Zeile {row.LineNumber}: Dieser Stundenplan-Eintrag ist bereits vorhanden."
                    );
                    continue;
                }

                entries.Add(entry);
            }

            return new TimetableCsvValidationResult
            {
                Entries = entries,
                Errors = errors
            };
        }

        private static string CreateEntryKey(StundenplanModel entry)
        {
            return string.Join(
                "|",
                entry.KursId,
                entry.Wochentag,
                entry.Startzeit.ToString("c", CultureInfo.InvariantCulture),
                entry.Endzeit.ToString("c", CultureInfo.InvariantCulture),
                (entry.Raum ?? string.Empty).Trim()
            );
        }

        private static bool TryParseTime(string value, out TimeSpan time)
        {
            return TimeSpan.TryParse(
                value,
                CultureInfo.InvariantCulture,
                out time
            );
        }

        private static bool TryParseWochentag(
            string value,
            out byte weekday)
        {
            if (byte.TryParse(value, out byte numeric) &&
                numeric is >= 1 and <= 5)
            {
                weekday = numeric;
                return true;
            }

            weekday = value.Trim().ToLowerInvariant() switch
            {
                "montag" => 1,
                "dienstag" => 2,
                "mittwoch" => 3,
                "donnerstag" => 4,
                "freitag" => 5,
                _ => 0
            };

            return weekday != 0;
        }

        private static string WochentagName(byte weekday)
        {
            return weekday switch
            {
                1 => "Montag",
                2 => "Dienstag",
                3 => "Mittwoch",
                4 => "Donnerstag",
                5 => "Freitag",
                _ => weekday.ToString(CultureInfo.InvariantCulture)
            };
        }

        private static CsvTable ParseCsv(string csv)
        {
            using StringReader reader = new StringReader(csv ?? string.Empty);

            string? headerLine = null;
            int lineNumber = 0;

            while (headerLine == null)
            {
                string? line = reader.ReadLine();
                lineNumber++;

                if (line == null)
                {
                    throw new FormatException(
                        "Die Datei enthält keine Kopfzeile."
                    );
                }

                if (!string.IsNullOrWhiteSpace(line))
                {
                    headerLine = line;
                }
            }

            char delimiter = CountDelimiter(headerLine, ';') >=
                             CountDelimiter(headerLine, ',')
                ? ';'
                : ',';

            List<string> headers = ParseCsvLine(headerLine, delimiter);
            headers[0] = headers[0].TrimStart('\uFEFF');

            List<CsvRow> rows = new List<CsvRow>();
            string? currentLine;

            while ((currentLine = reader.ReadLine()) != null)
            {
                lineNumber++;

                if (!string.IsNullOrWhiteSpace(currentLine))
                {
                    rows.Add(new CsvRow
                    {
                        LineNumber = lineNumber,
                        Values = ParseCsvLine(currentLine, delimiter)
                    });
                }
            }

            return new CsvTable
            {
                Headers = headers,
                Rows = rows
            };
        }

        private static int CountDelimiter(string line, char delimiter)
        {
            int count = 0;
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == '"')
                {
                    if (inQuotes &&
                        i + 1 < line.Length &&
                        line[i + 1] == '"')
                    {
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (!inQuotes && line[i] == delimiter)
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
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (!inQuotes && current == delimiter)
                {
                    values.Add(field.ToString());
                    field.Clear();
                }
                else
                {
                    field.Append(current);
                }
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
            params string[] names)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                if (names.Any(name => string.Equals(
                        headers[i].Trim(),
                        name,
                        StringComparison.OrdinalIgnoreCase)))
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
