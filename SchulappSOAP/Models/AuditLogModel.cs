using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchulAppSOAP.Models
{
    [Table("AuditLog", Schema = "dbo")]
    public class AuditLogModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime TimestampUtc { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? UserRole { get; set; }

        [MaxLength(10)]
        public string Action { get; set; } = string.Empty;

        [MaxLength(128)]
        public string EntityType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? EntityId { get; set; }

        public string? Changes { get; set; }

        [MaxLength(50)]
        public string Source { get; set; } = string.Empty;
    }
}
