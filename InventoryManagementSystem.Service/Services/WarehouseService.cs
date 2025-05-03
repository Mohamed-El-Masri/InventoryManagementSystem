using AutoMapper;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Exceptions;

namespace InventoryManagementSystem.Service.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public WarehouseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
        {
            var warehouses = await _unitOfWork.Warehouses.GetAllAsync();
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto?> GetWarehouseByIdAsync(Guid id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse == null)
                return null;

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<WarehouseDto> CreateWarehouseAsync(WarehouseCreateDto warehouseDto, string userId)
        {
            var warehouse = _mapper.Map<Warehouse>(warehouseDto);
            warehouse.CreatedBy = userId;

            await _unitOfWork.Warehouses.AddAsync(warehouse);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<WarehouseDto> UpdateWarehouseAsync(Guid id, WarehouseUpdateDto warehouseDto, string userId)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse == null)
                throw new NotFoundException($"Warehouse with ID {id} not found");

            _mapper.Map(warehouseDto, warehouse);
            warehouse.ModifiedBy = userId;
            warehouse.ModifiedAt = DateTime.UtcNow;

            await _unitOfWork.Warehouses.UpdateAsync(warehouse);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task DeleteWarehouseAsync(Guid id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetByIdAsync(id);
            if (warehouse == null)
                throw new NotFoundException($"Warehouse with ID {id} not found");

            // Check if warehouse has products
            bool hasProducts = await _unitOfWork.Warehouses.HasProductsAsync(id);
            if (hasProducts)
                throw new InvalidOperationException("Cannot delete warehouse with associated products");

            await _unitOfWork.Warehouses.DeleteAsync(warehouse);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<WarehouseDto>> GetActiveWarehousesAsync()
        {
            var warehouses = await _unitOfWork.Warehouses.GetActiveWarehousesAsync();
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto> GetWarehouseWithProductsAsync(Guid id)
        {
            var warehouse = await _unitOfWork.Warehouses.GetWarehouseWithProductsAsync(id);
            if (warehouse == null)
                throw new NotFoundException($"Warehouse with ID {id} not found");

            var warehouseDto = _mapper.Map<WarehouseDto>(warehouse);
            warehouseDto.ProductCount = warehouse.Products.Count;

            return warehouseDto;
        }
    }
}
