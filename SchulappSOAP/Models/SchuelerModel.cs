using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulAppSOAP.Models
{
    [Table("Schueler", Schema = "dbo")]
    public class SchuelerModel
    {
        [Key]
        public int SchuelerId { get; set; }

        public string Name { get; set; } = string.Empty;

        public int KlasseId { get; set; }
    }
}