using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public interface IInventoryTransactionService
    {
        // Stock In/Out operations
        Task<InventoryTransactionDto> AddStockAsync(StockTransactionDto stockInDto, string userId);
        Task<InventoryTransactionDto> RemoveStockAsync(StockTransactionDto stockOutDto, string userId);
        
        // Stock Transfer
        Task<InventoryTransactionDto> TransferStockAsync(StockTransferDto transferDto, string userId);
        
        // Query methods
        Task<IEnumerable<InventoryTransactionDto>> GetTransactionsByProductIdAsync(Guid productId);
        Task<IEnumerable<InventoryTransactionDto>> GetTransactionsByWarehouseIdAsync(Guid warehouseId, bool isSource);
        Task<TransactionHistoryReportDto> GetTransactionHistoryAsync(TransactionReportQueryDto queryDto);
    }
}
