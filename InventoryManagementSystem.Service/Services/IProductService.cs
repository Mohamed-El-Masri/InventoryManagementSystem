using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(Guid id);
        Task<ProductDto> CreateProductAsync(ProductCreateDto productDto, string userId);
        Task<ProductDto> UpdateProductAsync(Guid id, ProductUpdateDto productDto, string userId);
        Task DeleteProductAsync(Guid id);
        Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
        Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(ProductCategory category);
        Task<IEnumerable<ProductDto>> GetProductsByWarehouseAsync(Guid warehouseId);
    }
}
