using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    // Verknüpft dieses Model mit dbo.Schueler
    [Table("Schueler", Schema = "dbo")]
    public class SchuelerModel
    {
        // Primärschlüssel des Schülers
        [Key]
        public int SchuelerId { get; set; }

        public string Name { get; set; } = "";

        // Fremdschlüssel zur Klasse
        public int? KlasseId { get; set; }

        // Verbindung zur Klasse über Entity Framework
        public KlasseModel? Klasse { get; set; }
    }
}