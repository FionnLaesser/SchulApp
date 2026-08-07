namespace SchulApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Hauptmenue hauptmenue = new Hauptmenue();
            hauptmenue.Show();

            Application.Run();
        }
    }
}