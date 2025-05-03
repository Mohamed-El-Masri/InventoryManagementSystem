using InventoryManagementSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string EntityType { get; set; } = string.Empty;
        
        [Required]
        public Guid EntityId { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;
        
        [Required]
        public string OldValues { get; set; } = string.Empty;
        
        [Required]
        public string NewValues { get; set; } = string.Empty;
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        public string? UserId { get; set; }
        
        [MaxLength(100)]
        public string? UserName { get; set; }
    }
}
