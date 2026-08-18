using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulAppSOAP.Models
{
    // Verknüpft das Model mit dbo.Lehrer
    [Table("Lehrer", Schema = "dbo")]
    public class LehrerModel
    {
        // Primärschlüssel
        [Key]
        public int LehrerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = "";

        // Email darf in der Datenbank leer bzw. NULL sein
        [MaxLength(255)]
        public string? Email { get; set; }

        // Telefonnummer darf ebenfalls NULL sein
        [MaxLength(50)]
        public string? Telefon { get; set; }

        // Klassen, bei denen dieser Lehrer Klassenlehrer ist
        public ICollection<KlasseModel> Klassen { get; set; }
            = new List<KlasseModel>();

        // Zusätzliche Informationen über diesen Lehrer
        public ICollection<LehrerInformationModel> Informationen { get; set; }
            = new List<LehrerInformationModel>();
    }
}