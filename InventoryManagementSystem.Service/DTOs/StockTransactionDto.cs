namespace InventoryManagementSystem.Service.DTOs
{
    public class StockTransactionDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public decimal? UnitPrice { get; set; }
    }

    public class StockTransferDto
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public Guid SourceWarehouseId { get; set; }
        public Guid DestinationWarehouseId { get; set; }
        public string? Notes { get; set; }
    }

    public class TransactionReportQueryDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? ProductId { get; set; }
        public Guid? WarehouseId { get; set; }
        public string? UserId { get; set; }
        public int? TransactionType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
