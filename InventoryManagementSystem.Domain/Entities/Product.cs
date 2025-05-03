using InventoryManagementSystem.Domain.Common;
using InventoryManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class Product : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(500)]
        public string? Description { get; set; }
        
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Range(0, int.MaxValue)]
        public int LowStockThreshold { get; set; }
        
        public string? SKU { get; set; }
        
        public string? Barcode { get; set; }
        
        public ProductCategory Category { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        [MaxLength(255)]
        public string? ImageUrl { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal Weight { get; set; }
        
        public string? Dimensions { get; set; }
        
        // Navigation properties
        public Guid? WarehouseId { get; set; }
        public virtual Warehouse? Warehouse { get; set; }
        
        public virtual ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();
    }
}
