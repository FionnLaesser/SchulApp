using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp
{
    public partial class Register : Form
    {
        public string RegisteredUsername { get; private set; } = string.Empty;

        public Register()
        {
            InitializeComponent();

            passwordText.UseSystemPasswordChar = true;
            repeatPasswordText.UseSystemPasswordChar = true;
        }

        public Register(Point position) : this()
        {
            StartPosition = FormStartPosition.Manual;
            Location = position;
        }

        private async void registerBtn_Click(object sender, EventArgs e)
        {
            string benutzername = userText.Text.Trim();
            string passwort = passwordText.Text;
            string passwortWiederholung = repeatPasswordText.Text;

            if (benutzername.Length < 3)
            {
                ShowValidation(
                    "Der Benutzername muss mindestens 3 Zeichen lang sein.",
                    userText
                );

                return;
            }

            if (passwort.Length < 8)
            {
                ShowValidation(
                    "Das Passwort muss mindestens 8 Zeichen lang sein.",
                    passwordText
                );

                return;
            }

            if (passwort != passwortWiederholung)
            {
                ShowValidation(
                    "Die beiden Passwörter stimmen nicht überein.",
                    repeatPasswordText
                );

                return;
            }

            SetBusy(true);

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                bool existiertBereits = await db.Benutzer
                    .AnyAsync(x => x.Benutzername == benutzername);

                if (existiertBereits)
                {
                    ShowValidation(
                        "Dieser Benutzername ist bereits vergeben.",
                        userText
                    );

                    return;
                }

                LoginBenutzer neuerBenutzer = new LoginBenutzer
                {
                    Benutzername = benutzername,

                    // Es wird nur der Passwort-Hash gespeichert.
                    PasswortHash = BCrypt.Net.BCrypt.HashPassword(passwort),

                    ErstelltAm = DateTime.UtcNow
                };

                db.Benutzer.Add(neuerBenutzer);

                await db.SaveChangesAsync();

                RegisteredUsername = benutzername;

                MessageBox.Show(
                    "Registrierung erfolgreich. Du kannst dich jetzt anmelden.",
                    "Registrierung",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (DbUpdateException)
            {
                MessageBox.Show(
                    "Der Benutzer konnte nicht gespeichert werden.",
                    "Registrierung fehlgeschlagen",
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

        private void backToLoginBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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

        private void SetBusy(bool busy)
        {
            registerBtn.Enabled = !busy;
            backToLoginBtn.Enabled = !busy;
            userText.Enabled = !busy;
            passwordText.Enabled = !busy;
            repeatPasswordText.Enabled = !busy;

            Cursor = busy
                ? Cursors.WaitCursor
                : Cursors.Default;
        }
    }
}