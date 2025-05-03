using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Repository.Interfaces
{
    public interface IInventoryTransactionRepository : IRepositoryBase<InventoryTransaction>
    {
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByProductIdAsync(Guid productId);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByWarehouseIdAsync(Guid warehouseId, bool isSource);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByUserIdAsync(string userId);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsByTypeAsync(TransactionType transactionType);
        Task<IEnumerable<InventoryTransaction>> GetTransactionsOlderThanAsync(DateTime date);
    }
}
