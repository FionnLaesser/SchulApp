using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SchulAppSOAP.Models
{
    [DataContract]
    [Table("Schueler", Schema = "dbo")]
    public class SchuelerModel
    {
        [Key]
        [DataMember]
        public int SchuelerId { get; set; }

        [DataMember]
        public string Name { get; set; } = string.Empty;

        [DataMember]
        public int KlasseId { get; set; }

        [NotMapped]
        [DataMember(EmitDefaultValue = false)]
        public string? AuditUserName { get; set; }

        [NotMapped]
        [DataMember(EmitDefaultValue = false)]
        public string? AuditUserRole { get; set; }
    }
}
