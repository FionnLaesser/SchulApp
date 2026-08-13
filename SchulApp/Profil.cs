using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;
using System.Net.Mail;
using System.IO;

namespace SchulApp
{
    public partial class Profil : CustomForm
    {
        private const long MaxProfilbildGroesse = 5L * 1024L * 1024L;
        private readonly int benutzerId;

        public Profil(int benutzerId)
        {
            this.benutzerId = benutzerId;

            InitializeComponent();
            ThemeManager.Anwenden(this);
            StandardProfilbildAnzeigen();
        }

        private async void Profil_Load(object sender, EventArgs e)
        {
            await ProfilLadenAsync();
        }

        private async Task ProfilLadenAsync()
        {
            SetBusy(true);

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                LoginBenutzer? benutzer = await db.Benutzer
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.Id == benutzerId);

                if (benutzer == null)
                {
                    MessageBox.Show(
                        "Das Benutzerprofil konnte nicht gefunden werden.",
                        "Profil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    Close();
                    return;
                }

                userText.Text = benutzer.Benutzername;
                vornameText.Text = benutzer.Vorname ?? string.Empty;
                nachnameText.Text = benutzer.Nachname ?? string.Empty;
                emailText.Text = benutzer.Email ?? string.Empty;
                rolleText.Text = RolleAnzeigen(benutzer.Rolle);
                erstelltAmValueLabel.Text = benutzer.ErstelltAm
                    .ToLocalTime()
                    .ToString("dd.MM.yyyy HH:mm");
                profilNameLabel.Text = benutzer.Benutzername;

                ProfilbildAnzeigen(benutzer.Profilbild);
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Das Profil konnte nicht geladen werden.",
                    "Datenbankfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!IsDisposed)
                {
                    SetBusy(false);
                }
            }
        }

        private async void speichernBtn_Click(object sender, EventArgs e)
        {
            string benutzername = userText.Text.Trim();
            string vorname = vornameText.Text.Trim();
            string nachname = nachnameText.Text.Trim();
            string email = emailText.Text.Trim();

            if (benutzername.Length < 3 || benutzername.Length > 50)
            {
                ShowValidation(
                    "Der Benutzername muss zwischen 3 und 50 Zeichen lang sein.",
                    userText
                );
                return;
            }

            if (vorname.Length > 100)
            {
                ShowValidation(
                    "Der Vorname darf höchstens 100 Zeichen lang sein.",
                    vornameText
                );
                return;
            }

            if (nachname.Length > 100)
            {
                ShowValidation(
                    "Der Nachname darf höchstens 100 Zeichen lang sein.",
                    nachnameText
                );
                return;
            }

            if (email.Length > 255 || !IstGueltigeEmail(email))
            {
                ShowValidation(
                    "Bitte gib eine gültige E-Mail-Adresse ein oder lasse das Feld leer.",
                    emailText
                );
                return;
            }

            SetBusy(true);

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                bool benutzernameBelegt = await db.Benutzer.AnyAsync(
                    x => x.Id != benutzerId &&
                         x.Benutzername == benutzername
                );

                if (benutzernameBelegt)
                {
                    ShowValidation(
                        "Dieser Benutzername wird bereits verwendet.",
                        userText
                    );
                    return;
                }

                LoginBenutzer? benutzer = await db.Benutzer
                    .SingleOrDefaultAsync(x => x.Id == benutzerId);

                if (benutzer == null)
                {
                    MessageBox.Show(
                        "Das Benutzerprofil konnte nicht gefunden werden.",
                        "Profil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                benutzer.Benutzername = benutzername;
                benutzer.Vorname = LeerZuNull(vorname);
                benutzer.Nachname = LeerZuNull(nachname);
                benutzer.Email = LeerZuNull(email);

                await db.SaveChangesAsync();

                profilNameLabel.Text = benutzername;

                MessageBox.Show(
                    "Dein Profil wurde gespeichert.",
                    "Profil",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Das Profil konnte nicht gespeichert werden. Prüfe bitte deine Eingaben.",
                    "Profil",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Die Verbindung zur Datenbank ist fehlgeschlagen.",
                    "Datenbankfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!IsDisposed)
                {
                    SetBusy(false);
                }
            }
        }

        private async void profilbildAendernBtn_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Profilbild auswählen",
                Filter = "Bilddateien (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg",
                CheckFileExists = true,
                Multiselect = false
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            FileInfo datei = new FileInfo(dialog.FileName);

            if (datei.Length > MaxProfilbildGroesse)
            {
                MessageBox.Show(
                    "Das Profilbild darf maximal 5 MB gross sein.",
                    "Profilbild",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            SetBusy(true);

            try
            {
                byte[] bildDaten = await File.ReadAllBytesAsync(dialog.FileName);

                if (!IstGueltigesBild(bildDaten))
                {
                    MessageBox.Show(
                        "Die ausgewählte Datei ist kein gültiges Bild.",
                        "Profilbild",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                await using SchulAppContext db = new SchulAppContext();

                LoginBenutzer? benutzer = await db.Benutzer
                    .SingleOrDefaultAsync(x => x.Id == benutzerId);

                if (benutzer == null)
                {
                    MessageBox.Show(
                        "Das Benutzerprofil konnte nicht gefunden werden.",
                        "Profil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                benutzer.Profilbild = bildDaten;
                await db.SaveChangesAsync();

                ProfilbildAnzeigen(bildDaten);

                MessageBox.Show(
                    "Dein Profilbild wurde gespeichert.",
                    "Profilbild",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Das Profilbild konnte nicht gespeichert werden.",
                    "Profilbild",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!IsDisposed)
                {
                    SetBusy(false);
                }
            }
        }

        private async void profilbildEntfernenBtn_Click(object sender, EventArgs e)
        {
            DialogResult bestaetigung = MessageBox.Show(
                "Möchtest du dein Profilbild wirklich entfernen?",
                "Profilbild entfernen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (bestaetigung != DialogResult.Yes)
            {
                return;
            }

            SetBusy(true);

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                LoginBenutzer? benutzer = await db.Benutzer
                    .SingleOrDefaultAsync(x => x.Id == benutzerId);

                if (benutzer == null)
                {
                    MessageBox.Show(
                        "Das Benutzerprofil konnte nicht gefunden werden.",
                        "Profil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                benutzer.Profilbild = null;
                await db.SaveChangesAsync();

                StandardProfilbildAnzeigen();
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Das Profilbild konnte nicht entfernt werden.",
                    "Profilbild",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!IsDisposed)
                {
                    SetBusy(false);
                }
            }
        }

        private async void passwortAendernBtn_Click(object sender, EventArgs e)
        {
            string aktuellesPasswort = aktuellesPasswortText.Text;
            string neuesPasswort = neuesPasswortText.Text;
            string wiederholung = passwortWiederholenText.Text;

            if (string.IsNullOrWhiteSpace(aktuellesPasswort))
            {
                ShowValidation(
                    "Bitte gib dein aktuelles Passwort ein.",
                    aktuellesPasswortText
                );
                return;
            }

            if (neuesPasswort.Length < 8)
            {
                ShowValidation(
                    "Das neue Passwort muss mindestens 8 Zeichen lang sein.",
                    neuesPasswortText
                );
                return;
            }

            if (neuesPasswort != wiederholung)
            {
                ShowValidation(
                    "Die neuen Passwörter stimmen nicht überein.",
                    passwortWiederholenText
                );
                return;
            }

            SetBusy(true);

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                LoginBenutzer? benutzer = await db.Benutzer
                    .SingleOrDefaultAsync(x => x.Id == benutzerId);

                if (benutzer == null)
                {
                    MessageBox.Show(
                        "Das Benutzerprofil konnte nicht gefunden werden.",
                        "Profil",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                bool aktuellesPasswortKorrekt = BCrypt.Net.BCrypt.Verify(
                    aktuellesPasswort,
                    benutzer.PasswortHash
                );

                if (!aktuellesPasswortKorrekt)
                {
                    ShowValidation(
                        "Das aktuelle Passwort ist falsch.",
                        aktuellesPasswortText
                    );
                    aktuellesPasswortText.SelectAll();
                    return;
                }

                benutzer.PasswortHash = BCrypt.Net.BCrypt.HashPassword(
                    neuesPasswort
                );

                await db.SaveChangesAsync();

                aktuellesPasswortText.Clear();
                neuesPasswortText.Clear();
                passwortWiederholenText.Clear();

                MessageBox.Show(
                    "Dein Passwort wurde geändert.",
                    "Profil",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception)
            {
                MessageBox.Show(
                    "Das Passwort konnte nicht geändert werden.",
                    "Datenbankfehler",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                if (!IsDisposed)
                {
                    SetBusy(false);
                }
            }
        }

        private void zurueckBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ProfilbildAnzeigen(byte[]? bildDaten)
        {
            if (bildDaten == null || bildDaten.Length == 0)
            {
                StandardProfilbildAnzeigen();
                return;
            }

            try
            {
                using MemoryStream stream = new MemoryStream(bildDaten);
                using Image original = Image.FromStream(stream);

                SetProfilbild(new Bitmap(original));

                pictureBoxForUpload.Visible = false;
            }
            catch (ArgumentException)
            {
                StandardProfilbildAnzeigen();
            }
        }

        private void StandardProfilbildAnzeigen()
        {
            SetProfilbild(null);

            pictureBoxForUpload.BackColor = Color.Transparent;
            pictureBoxForUpload.Image = Image.FromFile("images/upload.png");
            pictureBoxForUpload.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBoxForUpload.Visible = true;
            pictureBoxForUpload.BringToFront();
        }

        private void SetProfilbild(Image? neuesBild)
        {
            Image? altesBild = profilPicture.Image;
            profilPicture.Image = neuesBild;
            altesBild?.Dispose();
        }

        private static bool IstGueltigesBild(byte[] bildDaten)
        {
            try
            {
                using MemoryStream stream = new MemoryStream(bildDaten);
                using Image image = Image.FromStream(
                    stream,
                    useEmbeddedColorManagement: false,
                    validateImageData: true
                );

                return image.Width > 0 && image.Height > 0;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        private void SetBusy(bool busy)
        {
            speichernBtn.Enabled = !busy;
            profilbildAendernBtn.Enabled = !busy;
            profilbildEntfernenBtn.Enabled = !busy;
            passwortAendernBtn.Enabled = !busy;
            userText.Enabled = !busy;
            vornameText.Enabled = !busy;
            nachnameText.Enabled = !busy;
            emailText.Enabled = !busy;
            aktuellesPasswortText.Enabled = !busy;
            neuesPasswortText.Enabled = !busy;
            passwortWiederholenText.Enabled = !busy;

            Cursor = busy
                ? Cursors.WaitCursor
                : Cursors.Default;
        }

        private static bool IstGueltigeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            try
            {
                MailAddress adresse = new MailAddress(email);
                return adresse.Address.Equals(
                    email,
                    StringComparison.OrdinalIgnoreCase
                );
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private static string? LeerZuNull(string text)
        {
            return string.IsNullOrWhiteSpace(text)
                ? null
                : text;
        }

        private static string RolleAnzeigen(string rolle)
        {
            return rolle == LoginBenutzer.RolleSchueler
                ? "Schüler"
                : rolle;
        }

        private static void ShowValidation(
            string text,
            Control control)
        {
            MessageBox.Show(
                text,
                "Eingabe prüfen",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            control.Focus();
        }

        private void pictureBoxForUpload_Click(object sender, EventArgs e)
        {
            profilbildAendernBtn_Click(sender, e);
        }
    }
}
