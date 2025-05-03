using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Service.Services;
using Microsoft.Extensions.Logging;

namespace InventoryManagementSystem.Service.BackgroundJobs
{
    public class InventoryJobs
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly ILogger<InventoryJobs> _logger;

        public InventoryJobs(
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            ILogger<InventoryJobs> logger)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task CheckLowStockProductsAsync()
        {
            try
            {
                _logger.LogInformation("Starting low stock products check");
                var lowStockProducts = await _unitOfWork.Products.GetLowStockProductsAsync();
                
                foreach (var product in lowStockProducts)
                {
                    _logger.LogInformation($"Product '{product.Name}' has low stock. Current quantity: {product.Quantity}, Threshold: {product.LowStockThreshold}");
                    await _notificationService.CreateLowStockNotificationAsync(product);
                }
                
                _logger.LogInformation($"Low stock check completed. Found {lowStockProducts.Count()} products below threshold.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking low stock products");
            }
        }

        public async Task ArchiveOldTransactionsAsync(int olderThanDays)
        {
            try
            {
                _logger.LogInformation($"Starting archival of transactions older than {olderThanDays} days");
                var cutoffDate = DateTime.UtcNow.AddDays(-olderThanDays);
                
                var oldTransactions = await _unitOfWork.InventoryTransactions.GetTransactionsOlderThanAsync(cutoffDate);
                _logger.LogInformation($"Found {oldTransactions.Count()} transactions to archive");
                
                int archiveCount = 0;
                foreach (var transaction in oldTransactions)
                {
                    await _unitOfWork.TransactionArchives.ArchiveTransactionAsync(transaction);
                    await _unitOfWork.InventoryTransactions.HardDeleteAsync(transaction);
                    
                    archiveCount++;
                    
                    if (archiveCount % 100 == 0)
                    {
                        await _unitOfWork.CompleteAsync();
                        _logger.LogInformation($"{archiveCount} transactions archived so far");
                    }
                }
                
                await _unitOfWork.CompleteAsync();
                _logger.LogInformation($"Transaction archiving completed. Successfully archived {archiveCount} transactions.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while archiving old transactions");
                throw;
            }
        }
    }
}
