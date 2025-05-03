using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public interface IReportService
    {
        Task<LowStockReportDto> GetLowStockReportAsync();
        Task<TransactionHistoryReportDto> GetTransactionHistoryReportAsync(TransactionReportQueryDto queryDto);
    }
}
