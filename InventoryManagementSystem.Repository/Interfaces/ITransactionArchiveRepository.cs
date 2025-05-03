using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Repository.Interfaces
{
    public interface ITransactionArchiveRepository : IRepositoryBase<TransactionArchive>
    {
        Task<IEnumerable<TransactionArchive>> GetArchivesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<TransactionArchive>> GetArchivesByProductIdAsync(Guid productId);
        Task<TransactionArchive> ArchiveTransactionAsync(InventoryTransaction transaction);
    }
}
