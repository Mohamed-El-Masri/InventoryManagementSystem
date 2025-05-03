using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        {
            return await _dbContext.Products
                .Where(p => p.Quantity <= p.LowStockThreshold && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory category)
        {
            return await _dbContext.Products
                .Where(p => p.Category == category && p.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByWarehouseAsync(Guid warehouseId)
        {
            return await _dbContext.Products
                .Where(p => p.WarehouseId == warehouseId && p.IsActive)
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithTransactionsAsync(Guid id)
        {
            return await _dbContext.Products
                .Include(p => p.InventoryTransactions)
                .ThenInclude(t => t.SourceWarehouse)
                .Include(p => p.InventoryTransactions)
                .ThenInclude(t => t.DestinationWarehouse)
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }
    }
}
