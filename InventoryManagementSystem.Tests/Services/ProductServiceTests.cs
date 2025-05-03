using AutoMapper;
using FluentAssertions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Services;
using Moq;
using Xunit;

namespace InventoryManagementSystem.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockProductRepository = new Mock<IProductRepository>();

            _mockUnitOfWork.Setup(uow => uow.Products).Returns(_mockProductRepository.Object);

            _productService = new ProductService(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetAllProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Test Product 1", Price = 10.99m },
                new Product { Id = Guid.NewGuid(), Name = "Test Product 2", Price = 20.99m }
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto { Id = products[0].Id, Name = products[0].Name, Price = products[0].Price },
                new ProductDto { Id = products[1].Id, Name = products[1].Name, Price = products[1].Price }
            };

            _mockProductRepository.Setup(repo => repo.GetAllAsync(
                    It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>()))
                .ReturnsAsync(products);

            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(products)).Returns(productDtos);

            // Act
            var result = await _productService.GetAllProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(productDtos);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Price = 10.99m };
            var productDto = new ProductDto { Id = productId, Name = "Test Product", Price = 10.99m };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product)).Returns(productDto);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(productDto);
        }

        [Fact]
        public async Task GetProductByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync((Product?)null);

            // Act
            var result = await _productService.GetProductByIdAsync(productId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateProductAsync_ShouldAddProductAndReturnDto()
        {
            // Arrange
            var userId = "test-user";
            var productCreateDto = new ProductCreateDto { Name = "New Product", Price = 15.99m };
            var product = new Product { Id = Guid.NewGuid(), Name = "New Product", Price = 15.99m };
            var productDto = new ProductDto { Id = product.Id, Name = "New Product", Price = 15.99m };

            _mockMapper.Setup(mapper => mapper.Map<Product>(productCreateDto)).Returns(product);
            _mockMapper.Setup(mapper => mapper.Map<ProductDto>(product)).Returns(productDto);
            _mockProductRepository.Setup(repo => repo.AddAsync(It.IsAny<Product>())).ReturnsAsync(product);

            // Act
            var result = await _productService.CreateProductAsync(productCreateDto, userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(productDto);
            product.CreatedBy.Should().Be(userId);
            _mockProductRepository.Verify(repo => repo.AddAsync(It.IsAny<Product>()), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task GetLowStockProductsAsync_ShouldReturnLowStockProducts()
        {
            // Arrange
            var lowStockProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Low Stock Product 1", Quantity = 5, LowStockThreshold = 10 },
                new Product { Id = Guid.NewGuid(), Name = "Low Stock Product 2", Quantity = 3, LowStockThreshold = 5 }
            };

            var lowStockProductDtos = new List<ProductDto>
            {
                new ProductDto { Id = lowStockProducts[0].Id, Name = lowStockProducts[0].Name, Quantity = lowStockProducts[0].Quantity },
                new ProductDto { Id = lowStockProducts[1].Id, Name = lowStockProducts[1].Name, Quantity = lowStockProducts[1].Quantity }
            };

            _mockProductRepository.Setup(repo => repo.GetLowStockProductsAsync()).ReturnsAsync(lowStockProducts);
            _mockMapper.Setup(mapper => mapper.Map<IEnumerable<ProductDto>>(lowStockProducts)).Returns(lowStockProductDtos);

            // Act
            var result = await _productService.GetLowStockProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(lowStockProductDtos);
        }
    }
}
