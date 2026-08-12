namespace SchulApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Könnte optionale Einstellungen aus der .env Datei. Wird gerade nicht benutzt
            // DotNetEnv.Env.TraversePath().Load();

            using LoadingScreen loadingScreen = new LoadingScreen();

            loadingScreen.Icon = Icon.ExtractAssociatedIcon(
                Application.ExecutablePath
            );

            Application.Run(loadingScreen);

            using Login login = new Login();

            login.Icon = Icon.ExtractAssociatedIcon(
                Application.ExecutablePath
            );

            if (login.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            ThemeManager.Laden();

            Icon? appIcon =
                Icon.ExtractAssociatedIcon(
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

            Application.Run(new Hauptmenue(login.AngemeldeteRolle));
        }
    }
}