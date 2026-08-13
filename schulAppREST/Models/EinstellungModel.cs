using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    // Verknüpft diese Klasse mit der Tabelle "Einstellung" in der Datenbank.
    // Die Id entspricht gleichzeitig der Id des Benutzers, dem die Einstellung gehört.
    [Table("Einstellung", Schema = "dbo")]
    public class EinstellungModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int HintergrundFarbe { get; set; }

        public int TextFarbe { get; set; }
    }
}
