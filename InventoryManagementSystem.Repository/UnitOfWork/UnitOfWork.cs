using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Repository.Repositories;

namespace InventoryManagementSystem.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IProductRepository _productRepository;
        private IInventoryTransactionRepository _inventoryTransactionRepository;
        private IWarehouseRepository _warehouseRepository;
        private INotificationRepository _notificationRepository;
        private ITransactionArchiveRepository _transactionArchiveRepository;
        private IAuditLogRepository _auditLogRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products => _productRepository ??= new ProductRepository(_context);
        public IInventoryTransactionRepository InventoryTransactions => _inventoryTransactionRepository ??= new InventoryTransactionRepository(_context);
        public IWarehouseRepository Warehouses => _warehouseRepository ??= new WarehouseRepository(_context);
        public INotificationRepository Notifications => _notificationRepository ??= new NotificationRepository(_context);
        public ITransactionArchiveRepository TransactionArchives => _transactionArchiveRepository ??= new TransactionArchiveRepository(_context);
        public IAuditLogRepository AuditLogs => _auditLogRepository ??= new AuditLogRepository(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
