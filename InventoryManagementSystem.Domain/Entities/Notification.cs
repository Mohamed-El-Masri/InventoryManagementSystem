using InventoryManagementSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Domain.Entities
{
    public class Notification : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
        
        public bool IsRead { get; set; } = false;
        
        [Required]
        public DateTime NotificationDate { get; set; } = DateTime.UtcNow;
        
        // Foreign keys and navigation properties
        public Guid? ProductId { get; set; }
        public virtual Product? Product { get; set; }
        
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
