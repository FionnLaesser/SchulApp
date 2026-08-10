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

            string? adminPasswortHash =
                Environment.GetEnvironmentVariable("ADMIN_PASSWORD_HASH");

            if (string.IsNullOrEmpty(adminBenutzername) ||
                string.IsNullOrEmpty(adminPasswortHash))
            {
                MessageBox.Show(
                    "Admin-Zugangsdaten konnten nicht geladen werden."
                );

                return;
            }

            bool benutzernameKorrekt =
                benutzername == adminBenutzername;

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