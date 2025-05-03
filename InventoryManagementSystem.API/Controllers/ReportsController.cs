using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            IReportService reportService,
            ILogger<ReportsController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<LowStockReportDto>> GetLowStockReport()
        {
            try
            {
                var report = await _reportService.GetLowStockReportAsync();
                _logger.LogInformation($"Low stock report generated with {report.TotalCount} products");
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating low stock report");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpGet("transaction-history")]
        public async Task<ActionResult<TransactionHistoryReportDto>> GetTransactionHistoryReport([FromQuery] TransactionReportQueryDto queryDto)
        {
            try
            {
                var report = await _reportService.GetTransactionHistoryReportAsync(queryDto);
                _logger.LogInformation($"Transaction history report generated with {report.TotalCount} transactions");
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating transaction history report");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }
    }
}
