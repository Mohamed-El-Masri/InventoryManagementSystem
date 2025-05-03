using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Exceptions;
using InventoryManagementSystem.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IInventoryTransactionService _transactionService;
        private readonly ILogger<InventoryTransactionsController> _logger;

        public InventoryTransactionsController(
            IInventoryTransactionService transactionService,
            ILogger<InventoryTransactionsController> logger)
        {
            _transactionService = transactionService;
            _logger = logger;
        }

        [HttpPost("stock-in")]
        public async Task<ActionResult<InventoryTransactionDto>> AddStock([FromBody] StockTransactionDto stockInDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var transaction = await _transactionService.AddStockAsync(stockInDto, userId);
                _logger.LogInformation($"Stock added: {stockInDto.Quantity} units to product {stockInDto.ProductId} by user {userId}");
                return Ok(transaction);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding stock");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpPost("stock-out")]
        public async Task<ActionResult<InventoryTransactionDto>> RemoveStock([FromBody] StockTransactionDto stockOutDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var transaction = await _transactionService.RemoveStockAsync(stockOutDto, userId);
                _logger.LogInformation($"Stock removed: {stockOutDto.Quantity} units from product {stockOutDto.ProductId} by user {userId}");
                return Ok(transaction);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing stock");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpPost("transfer")]
        public async Task<ActionResult<InventoryTransactionDto>> TransferStock([FromBody] StockTransferDto transferDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var transaction = await _transactionService.TransferStockAsync(transferDto, userId);
                _logger.LogInformation($"Stock transferred: {transferDto.Quantity} units of product {transferDto.ProductId} from warehouse {transferDto.SourceWarehouseId} to warehouse {transferDto.DestinationWarehouseId} by user {userId}");
                return Ok(transaction);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error transferring stock");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<InventoryTransactionDto>>> GetTransactionsByProduct(Guid productId)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByProductIdAsync(productId);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpGet("warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<InventoryTransactionDto>>> GetTransactionsByWarehouse(Guid warehouseId, [FromQuery] bool isSource = true)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionsByWarehouseIdAsync(warehouseId, isSource);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transactions");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }
    }
}
