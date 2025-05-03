using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Repositories
{
    public class InventoryTransactionRepository : RepositoryBase<InventoryTransaction>, IInventoryTransactionRepository
    {
        public InventoryTransactionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByProductIdAsync(Guid productId)
        {
            return await _dbContext.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Include(t => t.User)
                .Where(t => t.ProductId == productId && !t.IsDeleted)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByWarehouseIdAsync(Guid warehouseId, bool isSource)
        {
            if (isSource)
            {
                return await _dbContext.InventoryTransactions
                    .Include(t => t.Product)
                    .Include(t => t.SourceWarehouse)
                    .Include(t => t.DestinationWarehouse)
                    .Include(t => t.User)
                    .Where(t => t.SourceWarehouseId == warehouseId && !t.IsDeleted)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();
            }
            else
            {
                return await _dbContext.InventoryTransactions
                    .Include(t => t.Product)
                    .Include(t => t.SourceWarehouse)
                    .Include(t => t.DestinationWarehouse)
                    .Include(t => t.User)
                    .Where(t => t.DestinationWarehouseId == warehouseId && !t.IsDeleted)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByUserIdAsync(string userId)
        {
            return await _dbContext.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Include(t => t.User)
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Include(t => t.User)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate && !t.IsDeleted)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsByTypeAsync(TransactionType transactionType)
        {
            return await _dbContext.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Include(t => t.User)
                .Where(t => t.TransactionType == transactionType && !t.IsDeleted)
                .OrderByDescending(t => t.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<InventoryTransaction>> GetTransactionsOlderThanAsync(DateTime date)
        {
            return await _dbContext.InventoryTransactions
                .Include(t => t.Product)
                .Include(t => t.SourceWarehouse)
                .Include(t => t.DestinationWarehouse)
                .Include(t => t.User)
                .Where(t => t.TransactionDate < date && !t.IsDeleted)
                .ToListAsync();
        }
    }
}
