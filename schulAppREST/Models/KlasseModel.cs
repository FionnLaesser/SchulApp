using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    // Verknüpft dieses Model mit der bestehenden Tabelle dbo.Klassen
    [Table("Klassen", Schema = "dbo")]
    public class KlasseModel
    {
        // Primärschlüssel der Klasse
        [Key]
        public int KlassenId { get; set; }

        // Klassenbezeichnung, zum Beispiel "AP23a"
        [Required]
        [MaxLength(50)]
        public string Bezeichnung { get; set; } = "";

        // Kann null sein, falls die Klasse keinen Klassenlehrer hat
        public int? KlassenlehrerId { get; set; }

        // Verbindung zum Lehrer über Entity Framework
        public LehrerModel? Klassenlehrer { get; set; }

        // Alle Schüler, die dieser Klasse zugeordnet sind
        public ICollection<SchuelerModel> Schueler { get; set; }
            = new List<SchuelerModel>();
    }
}