using InventoryManagementSystem.Domain.Common;
using InventoryManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class InventoryTransaction : BaseEntity
    {
        [Required]
        public TransactionType TransactionType { get; set; }
        
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        
        public string? ReferenceNumber { get; set; }
        
        // Foreign keys and navigation properties
        [Required]
        public Guid ProductId { get; set; }
        public virtual Product? Product { get; set; }
        
        public Guid? SourceWarehouseId { get; set; }
        [ForeignKey("SourceWarehouseId")]
        public virtual Warehouse? SourceWarehouse { get; set; }
        
        public Guid? DestinationWarehouseId { get; set; }
        [ForeignKey("DestinationWarehouseId")]
        public virtual Warehouse? DestinationWarehouse { get; set; }
        
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
    }
}
