namespace SchulApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            ThemeManager.Laden();

            Application.Run(new Hauptmenue());
        }
    }
}