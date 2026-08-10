using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    // Verknüpft dieses Model mit der Tabelle dbo.Kurse
    [Table("Kurse", Schema = "dbo")]
    public class KursModel
    {
        // Primärschlüssel des Kurses
        [Key]
        public int KursId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        // Fremdschlüssel zur Klasse
        public int KlasseId { get; set; }

        // Verknüpfte Klasse
        public KlasseModel Klasse { get; set; } = null!;

        // Fremdschlüssel zum Lehrer
        public int LehrerId { get; set; }

        // Verknüpfter Lehrer
        public LehrerModel Lehrer { get; set; } = null!;
    }
}