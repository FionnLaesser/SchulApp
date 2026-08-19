using SchulApp.Models;
using SchulApp.Services;

namespace SchulApp
{
    public partial class Hauptmenue
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            DataTransferNachRolleInitialisieren();
        }

        private void DataTransferNachRolleInitialisieren()
        {
            dataTransferBtn.Visible =
                DataTransferPermissionService.DarfDatenTransferVerwenden(
                    rolle
                );

            dataTransferBtn.Click -= dataTransferBtn_Click;
            dataTransferBtn.Click += RoleBasedDataTransferBtn_Click;

            dataTransferMenu?.Dispose();
            dataTransferMenu = new ContextMenuStrip();

            ToolStripMenuItem exportItem = new ToolStripMenuItem("Export")
            {
                Image = MenuBildLaden("export.png")
            };

            if (rolle == LoginBenutzer.RolleAdmin)
            {
                exportItem.DropDownItems.Add(
                    RoleExportDatensatzMenuErstellen(
                        "Schülerliste",
                        ExportDataSet.Schueler
                    )
                );
                exportItem.DropDownItems.Add(
                    RoleExportDatensatzMenuErstellen(
                        "Klassenliste",
                        ExportDataSet.Klassen
                    )
                );
                exportItem.DropDownItems.Add(
                    RoleExportDatensatzMenuErstellen(
                        "Stundenplan",
                        ExportDataSet.Stundenplan
                    )
                );
                exportItem.DropDownItems.Add(
                    RoleExportDatensatzMenuErstellen(
                        "Audit Log",
                        ExportDataSet.AuditLog
                    )
                );
            }
            else if (rolle == LoginBenutzer.RolleLehrer)
            {
                exportItem.DropDownItems.Add(
                    RoleExportDatensatzMenuErstellen(
                        "Schülerliste",
                        ExportDataSet.Schueler
                    )
                );
                exportItem.DropDownItems.Add(
                    RoleExportDatensatzMenuErstellen(
                        "Stundenplan",
                        ExportDataSet.Stundenplan
                    )
                );
            }
            else if (rolle == LoginBenutzer.RolleSchueler)
            {
                exportItem.DropDownItems.Add(
                    RoleExportDatensatzMenuErstellen(
                        "Mein Stundenplan",
                        ExportDataSet.Stundenplan
                    )
                );
            }

            if (exportItem.DropDownItems.Count > 0)
            {
                dataTransferMenu.Items.Add(exportItem);
            }

            ToolStripMenuItem importItem = new ToolStripMenuItem("Import")
            {
                Image = MenuBildLaden("Import.png")
            };

            if (DataTransferPermissionService.DarfSchuelerImportieren(rolle))
            {
                ToolStripMenuItem importStudentsItem = new ToolStripMenuItem(
                    "Schüler"
                );
                importStudentsItem.Click += async (_, _) =>
                    await RoleBasedSchuelerImportierenAsync();

                importItem.DropDownItems.Add(importStudentsItem);
            }

            if (DataTransferPermissionService.DarfStundenplanImportieren(rolle))
            {
                ToolStripMenuItem importTimetableItem = new ToolStripMenuItem(
                    "Stundenplan"
                );
                importTimetableItem.Click += async (_, _) =>
                    await RoleBasedStundenplanImportierenAsync();

                importItem.DropDownItems.Add(importTimetableItem);
            }

            if (importItem.DropDownItems.Count > 0)
            {
                dataTransferMenu.Items.Add(importItem);
            }

            ToolStripMenuItem templateItem = new ToolStripMenuItem("Template");

            if (DataTransferPermissionService.DarfSchuelerImportieren(rolle))
            {
                ToolStripMenuItem studentTemplateItem = new ToolStripMenuItem(
                    "Schüler-CSV-Importvorlage"
                );
                studentTemplateItem.Click += async (_, _) =>
                    await SchuelerImportVorlageSpeichernAsync();

                templateItem.DropDownItems.Add(studentTemplateItem);
            }

            if (templateItem.DropDownItems.Count > 0)
            {
                dataTransferMenu.Items.Add(templateItem);
            }
        }

        private ToolStripMenuItem RoleExportDatensatzMenuErstellen(
            string text,
            ExportDataSet dataSet)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            ToolStripMenuItem csvItem = new ToolStripMenuItem("CSV");
            ToolStripMenuItem pdfItem = new ToolStripMenuItem("PDF");

            csvItem.Click += async (_, _) =>
                await RoleBasedExportierenAsync(
                    dataSet,
                    ExportFileFormat.Csv
                );

            pdfItem.Click += async (_, _) =>
                await RoleBasedExportierenAsync(
                    dataSet,
                    ExportFileFormat.Pdf
                );

            item.DropDownItems.Add(csvItem);
            item.DropDownItems.Add(pdfItem);

            return item;
        }

        private void RoleBasedDataTransferBtn_Click(
            object? sender,
            EventArgs e)
        {
            if (dataTransferMenu == null)
            {
                return;
            }

            dataTransferMenu.Show(
                dataTransferBtn,
                new Point(0, dataTransferBtn.Height)
            );
        }

        private async Task RoleBasedExportierenAsync(
            ExportDataSet dataSet,
            ExportFileFormat format)
        {
            string extension = format == ExportFileFormat.Csv
                ? "csv"
                : "pdf";

            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Daten exportieren",
                Filter = format == ExportFileFormat.Csv
                    ? "CSV-Datei (*.csv)|*.csv"
                    : "PDF-Datei (*.pdf)|*.pdf",
                DefaultExt = extension,
                AddExtension = true,
                RestoreDirectory = true,
                FileName = $"{ExportDateiname(dataSet)}-{DateTime.Now:yyyyMMdd-HHmm}.{extension}"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                dataTransferBtn.Enabled = false;

                await RoleBasedDataTransferService.ExportAsync(
                    benutzerId,
                    dataSet,
                    format,
                    dialog.FileName
                );

                MessageBox.Show(
                    this,
                    "Export erfolgreich abgeschlossen.",
                    "Export",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Keine Berechtigung",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Export fehlgeschlagen:{Environment.NewLine}{ex.Message}",
                    "Exportfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                dataTransferBtn.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task RoleBasedSchuelerImportierenAsync()
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Schüler importieren - erforderliche Spalten: Name;KlasseId",
                Filter = "CSV-Datei (*.csv)|*.csv",
                CheckFileExists = true,
                Multiselect = false,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                dataTransferBtn.Enabled = false;

                CsvImportResult result =
                    await RoleBasedDataTransferService.ImportStudentsAsync(
                        benutzerId,
                        dialog.FileName
                    );

                if (result.Errors.Count > 0)
                {
                    ImportFehlerAnzeigen(
                        "CSV-Importfehler",
                        result.Errors
                    );
                    return;
                }

                MessageBox.Show(
                    this,
                    $"{result.ImportedCount} Schüler wurden erfolgreich importiert.",
                    "CSV-Import",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Keine Berechtigung",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Import fehlgeschlagen:{Environment.NewLine}{ex.Message}",
                    "Importfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                dataTransferBtn.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task RoleBasedStundenplanImportierenAsync()
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Stundenplan importieren - Spalten: Kurs-ID;Wochentag;Start;Ende;Raum",
                Filter = "CSV-Datei (*.csv)|*.csv",
                CheckFileExists = true,
                Multiselect = false,
                RestoreDirectory = true
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                dataTransferBtn.Enabled = false;

                CsvImportResult result =
                    await RoleBasedDataTransferService.ImportTimetableAsync(
                        benutzerId,
                        dialog.FileName
                    );

                if (result.Errors.Count > 0)
                {
                    ImportFehlerAnzeigen(
                        "Stundenplan-Importfehler",
                        result.Errors
                    );
                    return;
                }

                MessageBox.Show(
                    this,
                    $"{result.ImportedCount} Stundenplan-Einträge wurden erfolgreich importiert.",
                    "Stundenplan-Import",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(
                    this,
                    ex.Message,
                    "Keine Berechtigung",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    this,
                    $"Import fehlgeschlagen:{Environment.NewLine}{ex.Message}",
                    "Importfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                dataTransferBtn.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void ImportFehlerAnzeigen(
            string titel,
            IReadOnlyList<string> errors)
        {
            string errorText = string.Join(
                Environment.NewLine,
                errors.Take(15)
            );

            if (errors.Count > 15)
            {
                errorText +=
                    $"{Environment.NewLine}... und {errors.Count - 15} weitere Fehler.";
            }

            MessageBox.Show(
                this,
                "Import abgebrochen. Es wurden keine Daten gespeichert." +
                Environment.NewLine +
                Environment.NewLine +
                errorText,
                titel,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }
    }
}
