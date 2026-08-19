namespace SchulApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            // Könnte optionale Einstellungen aus der .env Datei laden. Wird gerade nicht benutzt.
            // DotNetEnv.Env.TraversePath().Load();

            Icon? appIcon = Icon.ExtractAssociatedIcon(
                Application.ExecutablePath
            );

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

            using (LoadingScreen loadingScreen = new LoadingScreen())
            {
                loadingScreen.Icon = appIcon;
                Application.Run(loadingScreen);
            }

            Point? letzteFensterPosition = null;

            while (true)
            {
                using Login login = new Login
                {
                    Icon = appIcon
                };

                if (letzteFensterPosition.HasValue)
                {
                    login.StartPosition = FormStartPosition.Manual;
                    login.Location = letzteFensterPosition.Value;
                }

                if (login.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                letzteFensterPosition = login.Location;

                ThemeManager.Laden(login.AngemeldeteBenutzerId);

                using Hauptmenue hauptmenue = new Hauptmenue(
                    login.AngemeldeteBenutzerId,
                    login.AngemeldeteRolle
                )
                {
                    Icon = appIcon,
                    StartPosition = FormStartPosition.Manual,
                    Location = letzteFensterPosition.Value
                };

                Application.Run(hauptmenue);

                letzteFensterPosition = hauptmenue.Location;

                if (!hauptmenue.AbmeldenAngefordert)
                {
                    return;
                }

                ThemeManager.SitzungBeenden();
            }
        }
    }
}
