using SchulApp.Properties;

namespace SchulApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            ThemeManager.Laden(); // Load the theme settings
            Icon? appIcon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);  // Extrahiert das Symbol aus der .exe Datei.

            Application.Idle += (_, _) => // Setzt das Symbol für alle offenen Forms, wenn die Anwendung im Leerlauf ist.
            {
                foreach (Form form in Application.OpenForms)
                {
                    if (appIcon != null)
                    {
                        form.Icon = appIcon;
                    }
                }
            };
            Application.Run(new LoadingScreen());
        }
    }
}