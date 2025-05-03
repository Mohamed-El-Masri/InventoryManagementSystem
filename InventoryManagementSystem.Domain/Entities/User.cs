using InventoryManagementSystem.Domain.Common;
using InventoryManagementSystem.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Domain.Entities
{
    public class User : IdentityUser
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        public UserRole Role { get; set; } = UserRole.Employee;
        
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string? CreatedBy { get; set; }
        
        public DateTime? LastLoginDate { get; set; }
        
        // Navigation property
        public virtual ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
