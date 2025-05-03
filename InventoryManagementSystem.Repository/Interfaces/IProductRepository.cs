using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Repository.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<IEnumerable<Product>> GetLowStockProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory category);
        Task<IEnumerable<Product>> GetProductsByWarehouseAsync(Guid warehouseId);
        Task<Product?> GetProductWithTransactionsAsync(Guid id);
    }
}
