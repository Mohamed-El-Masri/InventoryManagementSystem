using AutoMapper;
using FluentAssertions;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Exceptions;
using InventoryManagementSystem.Service.Services;
using Moq;
using Xunit;

namespace InventoryManagementSystem.Tests.Services
{
    public class InventoryTransactionServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IInventoryTransactionRepository> _mockTransactionRepository;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly InventoryTransactionService _transactionService;

        public InventoryTransactionServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockTransactionRepository = new Mock<IInventoryTransactionRepository>();
            _mockNotificationService = new Mock<INotificationService>();

            _mockUnitOfWork.Setup(uow => uow.Products).Returns(_mockProductRepository.Object);
            _mockUnitOfWork.Setup(uow => uow.InventoryTransactions).Returns(_mockTransactionRepository.Object);

            _transactionService = new InventoryTransactionService(_mockUnitOfWork.Object, _mockMapper.Object, _mockNotificationService.Object);
        }

        [Fact]
        public async Task AddStockAsync_WithValidData_ShouldIncreaseProductQuantity()
        {
            // Arrange
            var userId = "test-user";
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Quantity = 10 };
            var stockInDto = new StockTransactionDto { ProductId = productId, Quantity = 5 };
            var transaction = new InventoryTransaction { Id = Guid.NewGuid(), ProductId = productId, Quantity = 5, TransactionType = TransactionType.StockIn };
            var transactionDto = new InventoryTransactionDto { Id = transaction.Id, ProductId = productId, Quantity = 5, TransactionType = TransactionType.StockIn };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<InventoryTransaction>())).ReturnsAsync(transaction);
            _mockMapper.Setup(mapper => mapper.Map<InventoryTransactionDto>(transaction)).Returns(transactionDto);

            // Act
            var result = await _transactionService.AddStockAsync(stockInDto, userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(transactionDto);
            product.Quantity.Should().Be(15); // Original 10 + 5 added
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<InventoryTransaction>()), Times.Once);
            _mockProductRepository.Verify(repo => repo.UpdateAsync(product), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveStockAsync_WithEnoughStock_ShouldDecreaseProductQuantity()
        {
            // Arrange
            var userId = "test-user";
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Quantity = 10, LowStockThreshold = 5 };
            var stockOutDto = new StockTransactionDto { ProductId = productId, Quantity = 3 };
            var transaction = new InventoryTransaction { Id = Guid.NewGuid(), ProductId = productId, Quantity = 3, TransactionType = TransactionType.StockOut };
            var transactionDto = new InventoryTransactionDto { Id = transaction.Id, ProductId = productId, Quantity = 3, TransactionType = TransactionType.StockOut };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<InventoryTransaction>())).ReturnsAsync(transaction);
            _mockMapper.Setup(mapper => mapper.Map<InventoryTransactionDto>(transaction)).Returns(transactionDto);

            // Act
            var result = await _transactionService.RemoveStockAsync(stockOutDto, userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(transactionDto);
            product.Quantity.Should().Be(7); // Original 10 - 3 removed
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<InventoryTransaction>()), Times.Once);
            _mockProductRepository.Verify(repo => repo.UpdateAsync(product), Times.Once);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveStockAsync_NotEnoughStock_ShouldThrowException()
        {
            // Arrange
            var userId = "test-user";
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Quantity = 5 };
            var stockOutDto = new StockTransactionDto { ProductId = productId, Quantity = 10 }; // Trying to remove more than available

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _transactionService.RemoveStockAsync(stockOutDto, userId));
            _mockTransactionRepository.Verify(repo => repo.AddAsync(It.IsAny<InventoryTransaction>()), Times.Never);
            _mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);
        }

        [Fact]
        public async Task RemoveStockAsync_WithStockBelowThreshold_ShouldCreateNotification()
        {
            // Arrange
            var userId = "test-user";
            var productId = Guid.NewGuid();
            var product = new Product { Id = productId, Name = "Test Product", Quantity = 10, LowStockThreshold = 8 };
            var stockOutDto = new StockTransactionDto { ProductId = productId, Quantity = 3 };
            var transaction = new InventoryTransaction { Id = Guid.NewGuid(), ProductId = productId, Quantity = 3, TransactionType = TransactionType.StockOut };
            var transactionDto = new InventoryTransactionDto { Id = transaction.Id, ProductId = productId, Quantity = 3, TransactionType = TransactionType.StockOut };

            _mockProductRepository.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);
            _mockTransactionRepository.Setup(repo => repo.AddAsync(It.IsAny<InventoryTransaction>())).ReturnsAsync(transaction);
            _mockMapper.Setup(mapper => mapper.Map<InventoryTransactionDto>(transaction)).Returns(transactionDto);

            // Act
            var result = await _transactionService.RemoveStockAsync(stockOutDto, userId);

            // Assert
            product.Quantity.Should().Be(7); // Original 10 - 3 removed, now below threshold of 8
            _mockNotificationService.Verify(service => service.CreateLowStockNotificationAsync(product), Times.Once);
        }
    }
}
