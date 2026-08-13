using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    // Verknüpft dieses Model mit der Tabelle dbo.Stundenplan
    [Table("Stundenplan", Schema = "dbo")]
    public class StundenplanModel
    {
        // Primärschlüssel
        [Key]
        public int StundenplanId { get; set; }

        // Fremdschlüssel zum Kurs
        public int KursId { get; set; }

        // Verknüpfter Kurs
        public KursModel? Kurs { get; set; } = null!;

        // 1 = Montag, 2 = Dienstag usw...
        public byte Wochentag { get; set; }

        // Startzeit des Unterrichts
        public TimeSpan Startzeit { get; set; }

        // Endzeit des Unterrichts
        public TimeSpan Endzeit { get; set; }

        // Raum kann leer sein
        [MaxLength(50)]
        public string? Raum { get; set; }
    }
}