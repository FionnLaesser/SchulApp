using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    // Verknüpft diese Klasse mit der Tabelle "Einstellung" in der Datenbank
    [Table("Einstellung", Schema = "dbo")]
    public class Einstellung
    {
        // Definiert Id als Primärschlüssel der Tabelle
        [Key]
        public int Id { get; set; }

        public int HintergrundFarbe { get; set; }

        
        public int TextFarbe { get; set; }
    }
}