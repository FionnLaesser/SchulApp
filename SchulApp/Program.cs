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

            while (true)
            {
                using Login login = new Login
                {
                    Icon = appIcon
                };

                if (login.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                ThemeManager.Laden(login.AngemeldeteBenutzerId);

                using Hauptmenue hauptmenue = new Hauptmenue(
                    login.AngemeldeteRolle
                )
                {
                    Icon = appIcon
                };

                Application.Run(hauptmenue);

                if (!hauptmenue.AbmeldenAngefordert)
                {
                    return;
                }

                ThemeManager.SitzungBeenden();
            }
        }
    }
}
