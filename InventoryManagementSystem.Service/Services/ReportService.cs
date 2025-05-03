using AutoMapper;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IInventoryTransactionService _transactionService;

        public ReportService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IInventoryTransactionService transactionService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _transactionService = transactionService;
        }

        public async Task<LowStockReportDto> GetLowStockReportAsync()
        {
            var lowStockProducts = await _unitOfWork.Products.GetLowStockProductsAsync();
            
            var report = new LowStockReportDto
            {
                LowStockProducts = _mapper.Map<List<ProductDto>>(lowStockProducts),
                TotalCount = lowStockProducts.Count(),
                ReportGeneratedAt = DateTime.UtcNow
            };
            
            return report;
        }

        public async Task<TransactionHistoryReportDto> GetTransactionHistoryReportAsync(TransactionReportQueryDto queryDto)
        {
            return await _transactionService.GetTransactionHistoryAsync(queryDto);
        }
    }
}
