namespace SchulApp.Models
{
    public class LoginBenutzer
    {
        public int Id { get; set; }

        public string Benutzername { get; set; } = string.Empty;

        public string PasswortHash { get; set; } = string.Empty;

        public DateTime ErstelltAm { get; set; }
    }
}