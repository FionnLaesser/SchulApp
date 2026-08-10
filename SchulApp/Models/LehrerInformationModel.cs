using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    // Verknüpft das Model mit dbo.LehrerInformationen
    [Table("LehrerInformationen", Schema = "dbo")]
    public class LehrerInformationModel
    {
        // Primärschlüssel
        [Key]
        public int LehrerInformationId { get; set; }

        // Fremdschlüssel zum Lehrer
        public int LehrerId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Titel { get; set; } = "";

        [Required]
        [MaxLength(500)]
        public string Information { get; set; } = "";

        // Zugehöriger Lehrer
        public LehrerModel? Lehrer { get; set; }
    }
}