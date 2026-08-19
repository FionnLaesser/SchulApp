using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using SchulApp.Services;
using System.IO;

namespace SchulApp
{
    public partial class Hauptmenue : CustomForm
    {
        private readonly int benutzerId;
        private readonly string rolle;
        private ContextMenuStrip? dataTransferMenu;

        public bool AbmeldenAngefordert { get; private set; }

        public Hauptmenue(int benutzerId, string rolle)
        {
            this.benutzerId = benutzerId;
            this.rolle = rolle switch
            {
                LoginBenutzer.RolleAdmin => LoginBenutzer.RolleAdmin,
                LoginBenutzer.RolleLehrer => LoginBenutzer.RolleLehrer,
                LoginBenutzer.RolleSchueler => LoginBenutzer.RolleSchueler,
                _ => LoginBenutzer.RolleSchueler
            };

            InitializeComponent();
            ThemeManager.Anwenden(this);
            RechteAnwenden();
            hauptbildLaden();
            StandardProfilButtonBildLaden();
            DataTransferButtonBildLaden();
            DataTransferMenuErstellen();
        }

        private bool IstAdmin => rolle == LoginBenutzer.RolleAdmin;
        private bool IstLehrer => rolle == LoginBenutzer.RolleLehrer;

        private async void Hauptmenue_Shown(object sender, EventArgs e) => await ProfilButtonBildLadenAsync();

        private void RechteAnwenden()
        {
            if (IstAdmin)
            {
                subtitleLabel.Text = "Admin: Vollzugriff auf die Schulverwaltung.";
                return;
            }

            lehrerPage.Visible = false;
            klassenPage.Visible = false;
            kursePage.Visible = false;
            auditLogPage.Visible = false;
            dataTransferBtn.Visible = false;

            if (IstLehrer)
            {
                subtitleLabel.Text = "Lehrer: Schüler verwalten und Stundenpläne bearbeiten.";
                schuelerPage.Location = new Point(338, 231);
                stundenplanPage.Location = new Point(338, 290);
                return;
            }

            schuelerPage.Visible = false;
            subtitleLabel.Text = "Schüler: Stundenplan ansehen.";
            stundenplanPage.Location = new Point(338, 260);
        }

        private void schuelerPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin && !IstLehrer) return;
            OeffneBereich(new Schueler());
        }

        private void lehrerPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin) return;
            OeffneBereich(new Lehrer());
        }

        private void klassenPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin) return;
            OeffneBereich(new Klassen());
        }

        private void kursePage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin) return;
            OeffneBereich(new Kurse());
        }

        private void stundenplanPage_Click(object sender, EventArgs e) =>
            OeffneBereich(new Stundenplan(darfBearbeiten: IstAdmin || IstLehrer));

        private void auditLogPage_Click(object sender, EventArgs e)
        {
            if (!IstAdmin) return;
            OeffneBereich(new AuditLog());
        }

        private void hauptbildLaden() => hauptbild.Image = Image.FromFile("images/Hauptbild.png");

        private async Task ProfilButtonBildLadenAsync()
        {
            try
            {
                await using SchulAppContext db = new SchulAppContext();
                byte[]? profilbild = await db.Benutzer.AsNoTracking().Where(x => x.Id == benutzerId).Select(x => x.Profilbild).SingleOrDefaultAsync();
                if (profilbild == null || profilbild.Length == 0)
                {
                    StandardProfilButtonBildLaden();
                    return;
                }

                using MemoryStream stream = new MemoryStream(profilbild);
                using Image original = Image.FromStream(stream);
                SetProfilButtonBild(new Bitmap(original, new Size(34, 34)));
            }
            catch (Exception)
            {
                StandardProfilButtonBildLaden();
            }
        }

        private void StandardProfilButtonBildLaden()
        {
            string pfad = Path.Combine(AppContext.BaseDirectory, "images", "profilePicture.png");
            if (!File.Exists(pfad))
            {
                SetProfilButtonBild(null);
                profileBtn.Text = "Profil";
                return;
            }

            using Image original = Image.FromFile(pfad);
            SetProfilButtonBild(new Bitmap(original, new Size(30, 30)));
        }

        private void SetProfilButtonBild(Image? neuesBild)
        {
            Image? altesBild = profileBtn.Image;
            profileBtn.Image = neuesBild;
            profileBtn.ImageAlign = ContentAlignment.MiddleCenter;
            profileBtn.Text = neuesBild == null ? "Profil" : string.Empty;
            altesBild?.Dispose();
        }

        private void DataTransferButtonBildLaden()
        {
            string pfad = Path.Combine(AppContext.BaseDirectory, "images", "CSVandPDFicon.png");
            if (!File.Exists(pfad)) return;
            using Image original = Image.FromFile(pfad);
            dataTransferBtn.Image = new Bitmap(original, new Size(28, 28));
            dataTransferBtn.ImageAlign = ContentAlignment.MiddleLeft;
            dataTransferBtn.TextImageRelation = TextImageRelation.ImageBeforeText;
        }

        private void DataTransferMenuErstellen()
        {
            dataTransferMenu?.Dispose();
            dataTransferMenu = new ContextMenuStrip();

            ToolStripMenuItem exportItem = new ToolStripMenuItem("Exportieren")
            {
                Image = MenuBildLaden("export.png")
            };

            exportItem.DropDownItems.Add(ExportDatensatzMenuErstellen("Schülerliste", ExportDataSet.Schueler));
            exportItem.DropDownItems.Add(ExportDatensatzMenuErstellen("Klassenliste", ExportDataSet.Klassen));
            exportItem.DropDownItems.Add(ExportDatensatzMenuErstellen("Stundenplan", ExportDataSet.Stundenplan));
            exportItem.DropDownItems.Add(ExportDatensatzMenuErstellen("Audit Log", ExportDataSet.AuditLog));

            ToolStripMenuItem importItem = new ToolStripMenuItem("Schüler aus CSV importieren")
            {
                Image = MenuBildLaden("Import.png")
            };
            importItem.Click += async (_, _) => await SchuelerImportierenAsync();

            ToolStripMenuItem templateItem = new ToolStripMenuItem("CSV-Importvorlage herunterladen");
            templateItem.Click += async (_, _) => await SchuelerImportVorlageSpeichernAsync();

            dataTransferMenu.Items.Add(exportItem);
            dataTransferMenu.Items.Add(importItem);
            dataTransferMenu.Items.Add(templateItem);
        }

        private ToolStripMenuItem ExportDatensatzMenuErstellen(string text, ExportDataSet dataSet)
        {
            ToolStripMenuItem item = new ToolStripMenuItem(text);
            ToolStripMenuItem csvItem = new ToolStripMenuItem("CSV");
            ToolStripMenuItem pdfItem = new ToolStripMenuItem("PDF");
            csvItem.Click += async (_, _) => await ExportierenAsync(dataSet, ExportFileFormat.Csv);
            pdfItem.Click += async (_, _) => await ExportierenAsync(dataSet, ExportFileFormat.Pdf);
            item.DropDownItems.Add(csvItem);
            item.DropDownItems.Add(pdfItem);
            return item;
        }

        private static Image? MenuBildLaden(string dateiname)
        {
            string pfad = Path.Combine(AppContext.BaseDirectory, "images", dateiname);
            if (!File.Exists(pfad)) return null;
            using Image original = Image.FromFile(pfad);
            return new Bitmap(original, new Size(20, 20));
        }

        private void dataTransferBtn_Click(object sender, EventArgs e)
        {
            if (!IstAdmin || dataTransferMenu == null) return;
            dataTransferMenu.Show(dataTransferBtn, new Point(0, dataTransferBtn.Height));
        }

        private async Task ExportierenAsync(ExportDataSet dataSet, ExportFileFormat format)
        {
            string extension = format == ExportFileFormat.Csv ? "csv" : "pdf";
            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Daten exportieren",
                Filter = format == ExportFileFormat.Csv ? "CSV-Datei (*.csv)|*.csv" : "PDF-Datei (*.pdf)|*.pdf",
                DefaultExt = extension,
                AddExtension = true,
                RestoreDirectory = true,
                FileName = $"{ExportDateiname(dataSet)}-{DateTime.Now:yyyyMMdd-HHmm}.{extension}"
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                dataTransferBtn.Enabled = false;
                await DataTransferService.ExportAsync(dataSet, format, dialog.FileName);
                MessageBox.Show(this, "Export erfolgreich abgeschlossen.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"Export fehlgeschlagen:{Environment.NewLine}{exception.Message}", "Exportfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dataTransferBtn.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task SchuelerImportVorlageSpeichernAsync()
        {
            using SaveFileDialog dialog = new SaveFileDialog
            {
                Title = "Schüler-Importvorlage speichern",
                Filter = "CSV-Datei (*.csv)|*.csv",
                DefaultExt = "csv",
                AddExtension = true,
                RestoreDirectory = true,
                FileName = "schueler-import-vorlage.csv"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                dataTransferBtn.Enabled = false;
                await StudentImportTemplateService.SaveTemplateAsync(dialog.FileName);
                MessageBox.Show(
                    this,
                    "Die Importvorlage wurde gespeichert. Sie enthält die benötigten Spalten Name und KlasseId sowie eine Beispielzeile.",
                    "Importvorlage",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"Vorlage konnte nicht gespeichert werden:{Environment.NewLine}{exception.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dataTransferBtn.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task SchuelerImportierenAsync()
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Schüler importieren - erforderliche Spalten: Name;KlasseId",
                Filter = "CSV-Datei (*.csv)|*.csv",
                CheckFileExists = true,
                Multiselect = false,
                RestoreDirectory = true
            };
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                Cursor = Cursors.WaitCursor;
                dataTransferBtn.Enabled = false;
                CsvImportResult result = await DataTransferService.ImportStudentsAsync(dialog.FileName);
                if (result.Errors.Count > 0)
                {
                    string errorText = string.Join(Environment.NewLine, result.Errors.Take(15));
                    if (result.Errors.Count > 15) errorText += $"{Environment.NewLine}... und {result.Errors.Count - 15} weitere Fehler.";
                    MessageBox.Show(this, "Import abgebrochen. Es wurden keine Schüler gespeichert." + Environment.NewLine + Environment.NewLine + errorText, "CSV-Importfehler", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                MessageBox.Show(this, $"{result.ImportedCount} Schüler wurden erfolgreich importiert.", "CSV-Import", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, $"Import fehlgeschlagen:{Environment.NewLine}{exception.Message}", "Importfehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dataTransferBtn.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private static string ExportDateiname(ExportDataSet dataSet) => dataSet switch
        {
            ExportDataSet.Schueler => "schueler",
            ExportDataSet.Klassen => "klassen",
            ExportDataSet.Stundenplan => "stundenplan",
            ExportDataSet.AuditLog => "audit-log",
            _ => "export"
        };

        private void abmeldenBtn_Click(object sender, EventArgs e)
        {
            AbmeldenAngefordert = true;
            Close();
        }

        private void einstellungPage_Click(object sender, EventArgs e) => OeffneBereich(new Einstellungen());

        private async void profileBtn_Click(object sender, EventArgs e)
        {
            OeffneBereich(new Profil(benutzerId));
            await ProfilButtonBildLadenAsync();
        }

        private void pingPong_Click(object sender, EventArgs e) => OeffneBereich(new PingPong());
        private void bestenlistePage_Click(object sender, EventArgs e) => OeffneBereich(new Bestenliste());
    }
}
