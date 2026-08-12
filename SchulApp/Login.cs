using Microsoft.EntityFrameworkCore;
using SchulApp.Data;
using SchulApp.Models;

namespace SchulApp
{
    public partial class Login : Form
    {
        public string AngemeldeteRolle { get; private set; } =
            LoginBenutzer.RolleSchueler;
        public Login()
        {
            InitializeComponent();
            passwordText.UseSystemPasswordChar = true;
        }

        private async void loginBtn_Click(object sender, EventArgs e)
        {
            string benutzername = userText.Text.Trim();
            string passwort = passwordText.Text;

            if (string.IsNullOrWhiteSpace(benutzername) ||
                string.IsNullOrWhiteSpace(passwort))
            {
                MessageBox.Show(
                    "Bitte Benutzername und Passwort eingeben.",
                    "Login",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            SetBusy(true);

            try
            {
                await using SchulAppContext db = new SchulAppContext();

                // Der Benutzer wird nur gelesen und nicht verändert.
                LoginBenutzer? benutzer = await db.Benutzer
                    .AsNoTracking()
                    .SingleOrDefaultAsync(
                        x => x.Benutzername == benutzername
                    );

                bool loginKorrekt =
                    benutzer != null &&
                    BCrypt.Net.BCrypt.Verify(
                        passwort,
                        benutzer.PasswortHash
                    );

                if (!loginKorrekt)
                {
                    MessageBox.Show(
                        "Benutzername oder Passwort ist falsch.",
                        "Login fehlgeschlagen",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    passwordText.Clear();
                    passwordText.Focus();

                    return;
                }

                AngemeldeteRolle = benutzer!.Rolle switch
                {
                    LoginBenutzer.RolleAdmin => LoginBenutzer.RolleAdmin,
                    LoginBenutzer.RolleLehrer => LoginBenutzer.RolleLehrer,
                    LoginBenutzer.RolleSchueler => LoginBenutzer.RolleSchueler,
                    _ => LoginBenutzer.RolleSchueler
                };

                DialogResult = DialogResult.OK;
                Close();
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

        private void registerBtn_Click(object sender, EventArgs e)
        {
            Point aktuellePosition = Location;

            Hide();

            using Register register = new Register(aktuellePosition)
            {
                Icon = Icon
            };

            DialogResult result = register.ShowDialog();

            // Die Position des Register-Fensters wird übernommen.
            StartPosition = FormStartPosition.Manual;
            Location = register.Location;

            if (result == DialogResult.OK)
            {
                userText.Text = register.RegisteredUsername;
                passwordText.Clear();
            }

            Show();
            Activate();

            if (string.IsNullOrWhiteSpace(userText.Text))
            {
                userText.Focus();
            }
            else
            {
                passwordText.Focus();
            }
        }

        private void SetBusy(bool busy)
        {
            loginBtn.Enabled = !busy;
            registerBtn.Enabled = !busy;
            userText.Enabled = !busy;
            passwordText.Enabled = !busy;

            Cursor = busy
                ? Cursors.WaitCursor
                : Cursors.Default;
        }
    }
}