using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Repository.Interfaces
{
    public interface IWarehouseRepository : IRepositoryBase<Warehouse>
    {
        Task<IEnumerable<Warehouse>> GetActiveWarehousesAsync();
        Task<bool> HasProductsAsync(Guid warehouseId);
        Task<Warehouse?> GetWarehouseWithProductsAsync(Guid id);
    }
}
