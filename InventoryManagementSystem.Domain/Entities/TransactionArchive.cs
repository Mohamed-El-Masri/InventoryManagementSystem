using InventoryManagementSystem.Domain.Common;
using InventoryManagementSystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Domain.Entities
{
    public class TransactionArchive : BaseEntity
    {
        [Required]
        public TransactionType TransactionType { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }
        
        [MaxLength(500)]
        public string? Notes { get; set; }
        
        [Required]
        public DateTime TransactionDate { get; set; }
        
        public string? ReferenceNumber { get; set; }
        
        // Original IDs for reference
        public Guid OriginalTransactionId { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid? SourceWarehouseId { get; set; }
        public string? SourceWarehouseName { get; set; }
        public Guid? DestinationWarehouseId { get; set; }
        public string? DestinationWarehouseName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        
        [Required]
        public DateTime ArchivedDate { get; set; } = DateTime.UtcNow;
    }
}
