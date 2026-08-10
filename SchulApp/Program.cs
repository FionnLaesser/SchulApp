using SchulApp.Properties;

namespace SchulApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Lädt die .env Datei
            DotNetEnv.Env.TraversePath().Load();

            // Extrahiert das Symbol aus der .exe Datei
            Icon? appIcon =
                Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            // Setzt das Symbol für alle offenen Forms
            Application.Idle += (_, _) =>
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (appIcon != null)
                    {
                        form.Icon = appIcon;
                    }
                }
            };

            // Login wird zuerst angezeigt
            using Login login = new Login();

            // Nur bei erfolgreichem Login geht die App weiter
            if (login.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Einstellungen erst nach erfolgreichem Login laden
            ThemeManager.Laden();

            // Danach startet der LoadingScreen
            Application.Run(new LoadingScreen());
        }
    }
}