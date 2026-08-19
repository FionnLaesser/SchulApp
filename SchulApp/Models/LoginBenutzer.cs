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

        public string? Vorname { get; set; }

        public string? Nachname { get; set; }

        public string? Email { get; set; }

        public byte[]? Profilbild { get; set; }

        public int? SchuelerId { get; set; }

        public DateTime ErstelltAm { get; set; }
    }
}
