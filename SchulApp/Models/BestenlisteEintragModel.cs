namespace SchulApp.Models
{
    public class BestenlisteEintragModel
    {
        public int BenutzerId { get; set; }

        public string Benutzername { get; set; } = string.Empty;

        public int Siege { get; set; }

        public int Punkte { get; set; }

        public int ToreErzielt { get; set; }

        public int ToreKassiert { get; set; }

        public int Torverhaeltnis { get; set; }
    }
}
