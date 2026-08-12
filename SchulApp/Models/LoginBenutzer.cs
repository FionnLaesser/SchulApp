namespace SchulApp.Models
{
    public class LoginBenutzer
    {
        public const string RolleAdmin = "Admin";
        public const string RolleLehrer = "Lehrer";
        public const string RolleSchueler = "Schueler";

        public int Id { get; set; }

        public string Benutzername { get; set; } = string.Empty;

        public string PasswortHash { get; set; } = string.Empty;

        public string Rolle { get; set; } = RolleSchueler;

        public DateTime ErstelltAm { get; set; }
    }
}
