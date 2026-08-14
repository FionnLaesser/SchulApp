namespace SchulApp.Models
{
    public class PingPongSpiel
    {
        public int Id { get; set; }

        public int SpielerLinksId { get; set; }

        public int SpielerRechtsId { get; set; }

        public int ToreLinks { get; set; }

        public int ToreRechts { get; set; }

        public int GewinnerId { get; set; }

        public DateTime GespieltAm { get; set; }
    }
}
