using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
        Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id);
        Task<WarehouseDto> CreateWarehouseAsync(WarehouseCreateDto warehouseDto, string userId);
        Task<WarehouseDto> UpdateWarehouseAsync(Guid id, WarehouseUpdateDto warehouseDto, string userId);
        Task DeleteWarehouseAsync(Guid id);
        Task<IEnumerable<WarehouseDto>> GetActiveWarehousesAsync();
        Task<WarehouseDto> GetWarehouseWithProductsAsync(Guid id);
    }
}
