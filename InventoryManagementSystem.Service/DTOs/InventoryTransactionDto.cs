using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Service.DTOs
{
    public class InventoryTransactionDto
    {
        public Guid Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Notes { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public Guid? SourceWarehouseId { get; set; }
        public string? SourceWarehouseName { get; set; }
        public Guid? DestinationWarehouseId { get; set; }
        public string? DestinationWarehouseName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }

    public class InventoryTransactionCreateDto
    {
        public TransactionType TransactionType { get; set; }
        public int Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? Notes { get; set; }
        public string? ReferenceNumber { get; set; }
        public Guid ProductId { get; set; }
        public Guid? SourceWarehouseId { get; set; }
        public Guid? DestinationWarehouseId { get; set; }
    }
}
