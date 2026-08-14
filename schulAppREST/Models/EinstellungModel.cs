using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulApp.Models
{
    [Table("Einstellung", Schema = "dbo")]
    public class EinstellungModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        public int HintergrundFarbe { get; set; }

        public int TextFarbe { get; set; }

        public int BallSpeed { get; set; } = 6;
    }
}
