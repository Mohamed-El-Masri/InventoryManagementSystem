using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Repositories
{
    public class TransactionArchiveRepository : RepositoryBase<TransactionArchive>, ITransactionArchiveRepository
    {
        public TransactionArchiveRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TransactionArchive>> GetArchivesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.TransactionArchives
                .Where(ta => ta.TransactionDate >= startDate && ta.TransactionDate <= endDate && !ta.IsDeleted)
                .OrderByDescending(ta => ta.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionArchive>> GetArchivesByProductIdAsync(Guid productId)
        {
            return await _dbContext.TransactionArchives
                .Where(ta => ta.ProductId == productId && !ta.IsDeleted)
                .OrderByDescending(ta => ta.TransactionDate)
                .ToListAsync();
        }

        public async Task<TransactionArchive> ArchiveTransactionAsync(InventoryTransaction transaction)
        {
            var archive = new TransactionArchive
            {
                OriginalTransactionId = transaction.Id,
                TransactionType = transaction.TransactionType,
                Quantity = transaction.Quantity,
                UnitPrice = transaction.UnitPrice,
                Notes = transaction.Notes,
                TransactionDate = transaction.TransactionDate,
                ReferenceNumber = transaction.ReferenceNumber,
                ProductId = transaction.ProductId,
                ProductName = transaction.Product?.Name,
                SourceWarehouseId = transaction.SourceWarehouseId,
                SourceWarehouseName = transaction.SourceWarehouse?.Name,
                DestinationWarehouseId = transaction.DestinationWarehouseId,
                DestinationWarehouseName = transaction.DestinationWarehouse?.Name,
                UserId = transaction.UserId,
                UserName = transaction.User != null ? $"{transaction.User.FirstName} {transaction.User.LastName}" : null,
                CreatedBy = transaction.CreatedBy,
                CreatedAt = transaction.CreatedAt,
                ModifiedBy = transaction.ModifiedBy,
                ModifiedAt = transaction.ModifiedAt
            };

            await _dbContext.TransactionArchives.AddAsync(archive);
            return archive;
        }
    }
}
