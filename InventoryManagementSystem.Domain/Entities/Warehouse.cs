using InventoryManagementSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Domain.Entities
{
    public class Warehouse : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(200)]
        public string? Location { get; set; }
        
        [MaxLength(50)]
        public string? Code { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<InventoryTransaction> SourceTransactions { get; set; } = new List<InventoryTransaction>();
        public virtual ICollection<InventoryTransaction> DestinationTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
