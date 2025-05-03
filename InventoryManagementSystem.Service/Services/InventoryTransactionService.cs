using AutoMapper;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Domain.Enums;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Exceptions;

namespace InventoryManagementSystem.Service.Services
{
    public class InventoryTransactionService : IInventoryTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public InventoryTransactionService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<InventoryTransactionDto> AddStockAsync(StockTransactionDto stockInDto, string userId)
        {
            // Get the product
            var product = await _unitOfWork.Products.GetByIdAsync(stockInDto.ProductId);
            if (product == null)
                throw new NotFoundException($"Product with ID {stockInDto.ProductId} not found");

            // Create transaction
            var transaction = new InventoryTransaction
            {
                TransactionType = TransactionType.StockIn,
                ProductId = stockInDto.ProductId,
                Quantity = stockInDto.Quantity,
                UnitPrice = stockInDto.UnitPrice,
                Notes = stockInDto.Notes,
                UserId = userId,
                DestinationWarehouseId = product.WarehouseId,
                CreatedBy = userId
            };

            await _unitOfWork.InventoryTransactions.AddAsync(transaction);

            // Update product quantity
            product.Quantity += stockInDto.Quantity;
            product.ModifiedBy = userId;
            product.ModifiedAt = DateTime.UtcNow;
            await _unitOfWork.Products.UpdateAsync(product);

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<InventoryTransactionDto>(transaction);
        }

        public async Task<InventoryTransactionDto> RemoveStockAsync(StockTransactionDto stockOutDto, string userId)
        {
            // Get the product
            var product = await _unitOfWork.Products.GetByIdAsync(stockOutDto.ProductId);
            if (product == null)
                throw new NotFoundException($"Product with ID {stockOutDto.ProductId} not found");

            // Check if there's enough stock
            if (product.Quantity < stockOutDto.Quantity)
                throw new InvalidOperationException($"Not enough stock available for product '{product.Name}'. Available: {product.Quantity}, Requested: {stockOutDto.Quantity}");

            // Create transaction
            var transaction = new InventoryTransaction
            {
                TransactionType = TransactionType.StockOut,
                ProductId = stockOutDto.ProductId,
                Quantity = stockOutDto.Quantity,
                UnitPrice = stockOutDto.UnitPrice,
                Notes = stockOutDto.Notes,
                UserId = userId,
                SourceWarehouseId = product.WarehouseId,
                CreatedBy = userId
            };

            await _unitOfWork.InventoryTransactions.AddAsync(transaction);

            // Update product quantity
            product.Quantity -= stockOutDto.Quantity;
            product.ModifiedBy = userId;
            product.ModifiedAt = DateTime.UtcNow;
            await _unitOfWork.Products.UpdateAsync(product);

            await _unitOfWork.CompleteAsync();

            // Check if stock is below threshold after removal
            if (product.Quantity <= product.LowStockThreshold)
            {
                await _notificationService.CreateLowStockNotificationAsync(product);
            }

            return _mapper.Map<InventoryTransactionDto>(transaction);
        }

        public async Task<InventoryTransactionDto> TransferStockAsync(StockTransferDto transferDto, string userId)
        {
            // Get the product
            var product = await _unitOfWork.Products.GetByIdAsync(transferDto.ProductId);
            if (product == null)
                throw new NotFoundException($"Product with ID {transferDto.ProductId} not found");

            // Check if warehouses exist
            var sourceWarehouse = await _unitOfWork.Warehouses.GetByIdAsync(transferDto.SourceWarehouseId);
            if (sourceWarehouse == null)
                throw new NotFoundException($"Source warehouse with ID {transferDto.SourceWarehouseId} not found");

            var destWarehouse = await _unitOfWork.Warehouses.GetByIdAsync(transferDto.DestinationWarehouseId);
            if (destWarehouse == null)
                throw new NotFoundException($"Destination warehouse with ID {transferDto.DestinationWarehouseId} not found");

            // Check if the product is in the source warehouse
            if (product.WarehouseId != transferDto.SourceWarehouseId)
                throw new InvalidOperationException($"Product '{product.Name}' is not in the source warehouse");

            // Check if there's enough stock
            if (product.Quantity < transferDto.Quantity)
                throw new InvalidOperationException($"Not enough stock available for product '{product.Name}'. Available: {product.Quantity}, Requested: {transferDto.Quantity}");

            // Create transaction
            var transaction = new InventoryTransaction
            {
                TransactionType = TransactionType.Transfer,
                ProductId = transferDto.ProductId,
                Quantity = transferDto.Quantity,
                Notes = transferDto.Notes,
                UserId = userId,
                SourceWarehouseId = transferDto.SourceWarehouseId,
                DestinationWarehouseId = transferDto.DestinationWarehouseId,
                CreatedBy = userId
            };

            await _unitOfWork.InventoryTransactions.AddAsync(transaction);

            // Update product warehouse
            product.WarehouseId = transferDto.DestinationWarehouseId;
            product.ModifiedBy = userId;
            product.ModifiedAt = DateTime.UtcNow;
            await _unitOfWork.Products.UpdateAsync(product);

            await _unitOfWork.CompleteAsync();

            return _mapper.Map<InventoryTransactionDto>(transaction);
        }

        public async Task<IEnumerable<InventoryTransactionDto>> GetTransactionsByProductIdAsync(Guid productId)
        {
            var transactions = await _unitOfWork.InventoryTransactions.GetTransactionsByProductIdAsync(productId);
            return _mapper.Map<IEnumerable<InventoryTransactionDto>>(transactions);
        }

        public async Task<IEnumerable<InventoryTransactionDto>> GetTransactionsByWarehouseIdAsync(Guid warehouseId, bool isSource)
        {
            var transactions = await _unitOfWork.InventoryTransactions.GetTransactionsByWarehouseIdAsync(warehouseId, isSource);
            return _mapper.Map<IEnumerable<InventoryTransactionDto>>(transactions);
        }

        public async Task<TransactionHistoryReportDto> GetTransactionHistoryAsync(TransactionReportQueryDto queryDto)
        {
            IEnumerable<InventoryTransaction> transactions;

            if (queryDto.ProductId.HasValue)
            {
                transactions = await _unitOfWork.InventoryTransactions.GetTransactionsByProductIdAsync(queryDto.ProductId.Value);
            }
            else if (queryDto.WarehouseId.HasValue)
            {
                var sourceTransactions = await _unitOfWork.InventoryTransactions.GetTransactionsByWarehouseIdAsync(queryDto.WarehouseId.Value, true);
                var destinationTransactions = await _unitOfWork.InventoryTransactions.GetTransactionsByWarehouseIdAsync(queryDto.WarehouseId.Value, false);
                transactions = sourceTransactions.Concat(destinationTransactions);
            }
            else if (queryDto.StartDate.HasValue && queryDto.EndDate.HasValue)
            {
                transactions = await _unitOfWork.InventoryTransactions.GetTransactionsByDateRangeAsync(queryDto.StartDate.Value, queryDto.EndDate.Value);
            }
            else if (queryDto.UserId != null)
            {
                transactions = await _unitOfWork.InventoryTransactions.GetTransactionsByUserIdAsync(queryDto.UserId);
            }
            else if (queryDto.TransactionType.HasValue)
            {
                transactions = await _unitOfWork.InventoryTransactions.GetTransactionsByTypeAsync((TransactionType)queryDto.TransactionType.Value);
            }
            else
            {
                transactions = await _unitOfWork.InventoryTransactions.GetAllAsync(
                    orderBy: q => q.OrderByDescending(t => t.TransactionDate),
                    includeProperties: "Product,SourceWarehouse,DestinationWarehouse,User"
                );
            }

            // Filter by date range if provided
            if (queryDto.StartDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate >= queryDto.StartDate.Value);
            if (queryDto.EndDate.HasValue)
                transactions = transactions.Where(t => t.TransactionDate <= queryDto.EndDate.Value);

            // Convert to list for pagination
            var transactionsList = transactions.ToList();
            var totalCount = transactionsList.Count;
            
            // Apply pagination
            var pagedTransactions = transactionsList
                .Skip((queryDto.Page - 1) * queryDto.PageSize)
                .Take(queryDto.PageSize)
                .ToList();

            var result = new TransactionHistoryReportDto
            {
                Transactions = _mapper.Map<List<InventoryTransactionDto>>(pagedTransactions),
                TotalCount = totalCount,
                CurrentPage = queryDto.Page,
                PageSize = queryDto.PageSize,
                PageCount = (int)Math.Ceiling(totalCount / (double)queryDto.PageSize)
            };

            return result;
        }
    }
}
