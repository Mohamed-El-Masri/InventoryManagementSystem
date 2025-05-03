using InventoryManagementSystem.Repository.Interfaces;

namespace InventoryManagementSystem.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IInventoryTransactionRepository InventoryTransactions { get; }
        IWarehouseRepository Warehouses { get; }
        INotificationRepository Notifications { get; }
        ITransactionArchiveRepository TransactionArchives { get; }
        IAuditLogRepository AuditLogs { get; }
        Task<int> CompleteAsync();
    }
}
