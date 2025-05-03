using FluentAssertions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InventoryManagementSystem.Tests.Integration
{
    public class ProductRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ProductRepository _productRepository;

        public ProductRepositoryTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: $"ProductDb_{Guid.NewGuid()}")
                .Options;
            
            _dbContext = new ApplicationDbContext(options);
            _productRepository = new ProductRepository(_dbContext);
            
            // Seed test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var warehouse = new Warehouse { Id = Guid.NewGuid(), Name = "Test Warehouse" };
            _dbContext.Warehouses.Add(warehouse);

            var products = new List<Product>
            {
                new Product 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Product 1", 
                    Description = "Description 1", 
                    Price = 10.99m, 
                    Quantity = 20, 
                    LowStockThreshold = 5, 
                    Category = ProductCategory.Electronics,
                    WarehouseId = warehouse.Id
                },
                new Product 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Product 2", 
                    Description = "Description 2", 
                    Price = 15.99m, 
                    Quantity = 3, 
                    LowStockThreshold = 5, 
                    Category = ProductCategory.Electronics,
                    WarehouseId = warehouse.Id
                },
                new Product 
                { 
                    Id = Guid.NewGuid(), 
                    Name = "Product 3", 
                    Description = "Description 3", 
                    Price = 20.99m, 
                    Quantity = 10, 
                    LowStockThreshold = 15, 
                    Category = ProductCategory.HomeAndGarden,
                    WarehouseId = warehouse.Id,
                    IsDeleted = true // This should be filtered out by soft delete
                }
            };
            
            _dbContext.Products.AddRange(products);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllNonDeletedProducts()
        {
            // Act
            var products = await _productRepository.GetAllAsync();
            
            // Assert
            products.Should().NotBeNull();
            products.Should().HaveCount(2); // Only 2 products are not deleted
        }

        [Fact]
        public async Task GetLowStockProductsAsync_ShouldReturnOnlyProductsBelowThreshold()
        {
            // Act
            var lowStockProducts = await _productRepository.GetLowStockProductsAsync();
            
            // Assert
            lowStockProducts.Should().NotBeNull();
            lowStockProducts.Should().HaveCount(1);
            lowStockProducts.First().Name.Should().Be("Product 2");
        }

        [Fact]
        public async Task GetProductsByCategoryAsync_ShouldReturnProductsInSpecificCategory()
        {
            // Act
            var electronicsProducts = await _productRepository.GetProductsByCategoryAsync(ProductCategory.Electronics);
            var homeProducts = await _productRepository.GetProductsByCategoryAsync(ProductCategory.HomeAndGarden);
            
            // Assert
            electronicsProducts.Should().NotBeNull();
            electronicsProducts.Should().HaveCount(2);
            
            homeProducts.Should().NotBeNull();
            homeProducts.Should().BeEmpty(); // The HomeAndGarden product is marked as deleted
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewProduct()
        {
            // Arrange
            var newProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "New Test Product",
                Description = "New Description",
                Price = 25.99m,
                Quantity = 15,
                LowStockThreshold = 5,
                Category = ProductCategory.Books
            };
            
            // Act
            await _productRepository.AddAsync(newProduct);
            await _dbContext.SaveChangesAsync();
            
            // Assert
            var addedProduct = await _dbContext.Products.FindAsync(newProduct.Id);
            addedProduct.Should().NotBeNull();
            addedProduct!.Name.Should().Be("New Test Product");
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
