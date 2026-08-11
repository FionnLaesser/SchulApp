namespace SchulApp
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();

            passwordText.UseSystemPasswordChar = true;
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            string benutzername = userText.Text.Trim();
            string passwort = passwordText.Text;

            string? adminBenutzername =
                Environment.GetEnvironmentVariable("ADMIN_USERNAME");

            string? adminPasswort =
                Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

            if (string.IsNullOrEmpty(adminBenutzername) ||
                string.IsNullOrEmpty(adminPasswort))
            {
                MessageBox.Show(
                    "Admin-Zugangsdaten konnten nicht geladen werden."
                );

                return;
            }

            bool benutzernameKorrekt =
                benutzername == adminBenutzername;

            // Passwort aus der .env wird mit BCrypt gehasht
            string adminPasswortHash =
                BcryptHasher.HashPassword(adminPasswort);

            // Eingegebenes Passwort wird mit dem Hash verglichen
            bool passwortKorrekt =
                BCrypt.Net.BCrypt.Verify(
                    passwort,
                    adminPasswortHash
                );

            if (benutzernameKorrekt && passwortKorrekt)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(
                    "Benutzername oder Passwort ist falsch."
                );

                passwordText.Clear();
            }
        }
    }
}