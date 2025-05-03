using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Service.DTOs
{
    public class LowStockReportDto
    {
        public List<ProductDto> LowStockProducts { get; set; } = new List<ProductDto>();
        public int TotalCount { get; set; }
        public DateTime ReportGeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public class TransactionHistoryReportDto
    {
        public List<InventoryTransactionDto> Transactions { get; set; } = new List<InventoryTransactionDto>();
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public DateTime ReportGeneratedAt { get; set; } = DateTime.UtcNow;
    }
}
