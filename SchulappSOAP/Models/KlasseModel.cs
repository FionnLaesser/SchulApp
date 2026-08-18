using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulAppSOAP.Models
{
    [Table("Klassen", Schema = "dbo")]
    public class KlasseModel
    {
        [Key]
        public int KlassenId { get; set; }

        public string Bezeichnung { get; set; } = string.Empty;

        public int? KlassenlehrerId { get; set; }
    }
}