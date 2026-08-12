namespace SchulApp
{
    internal static class BenutzerSession
    {
        public static int BenutzerId { get; set; }

        public static string Benutzername { get; set; } = "";

        public static string Rolle { get; set; } = "";

        public static void Abmelden()
        {
            BenutzerId = 0;
            Benutzername = "";
            Rolle = "";
        }
    }
}