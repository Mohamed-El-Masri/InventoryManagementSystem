using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Repositories
{
    public class WarehouseRepository : RepositoryBase<Warehouse>, IWarehouseRepository
    {
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync()
        {
            return await _dbContext.Warehouses
                .Where(w => w.IsActive && !w.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> HasProductsAsync(Guid warehouseId)
        {
            return await _dbContext.Products
                .AnyAsync(p => p.WarehouseId == warehouseId && !p.IsDeleted);
        }

        public async Task<Warehouse?> GetWarehouseWithProductsAsync(Guid id)
        {
            return await _dbContext.Warehouses
                .Include(w => w.Products.Where(p => !p.IsDeleted))
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted);
        }
    }
}
