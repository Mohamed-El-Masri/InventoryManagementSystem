using AutoMapper;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Exceptions;

namespace InventoryManagementSystem.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync(
                includeProperties: "Warehouse");
            
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            
            if (product == null)
                return null;
                
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto productDto, string userId)
        {
            var product = _mapper.Map<Product>(productDto);
            product.CreatedBy = userId;
            
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateProductAsync(Guid id, ProductUpdateDto productDto, string userId)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found");
                
            _mapper.Map(productDto, product);
            product.ModifiedBy = userId;
            product.ModifiedAt = DateTime.UtcNow;
            
            await _unitOfWork.Products.UpdateAsync(product);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            
            if (product == null)
                throw new NotFoundException($"Product with ID {id} not found");
                
            await _unitOfWork.Products.DeleteAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
        {
            var products = await _unitOfWork.Products.GetLowStockProductsAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByCategoryAsync(ProductCategory category)
        {
            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(category);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<IEnumerable<ProductDto>> GetProductsByWarehouseAsync(Guid warehouseId)
        {
            var products = await _unitOfWork.Products.GetProductsByWarehouseAsync(warehouseId);
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }
    }
}
