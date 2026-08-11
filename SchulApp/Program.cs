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

            using LoadingScreen loadingScreen = new LoadingScreen();

            loadingScreen.Icon = Icon.ExtractAssociatedIcon(
                Application.ExecutablePath
            );

            Application.Run(loadingScreen);

            // Danach Login
            using Login login = new Login();
            // Icon setzen, weil der erste Application.Run bereits beendet wurde
            login.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);


            if (login.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Einstellungen laden
            ThemeManager.Laden();
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
            // Hauptprogramm starten
            Application.Run(new Hauptmenue());
        }
    }
}