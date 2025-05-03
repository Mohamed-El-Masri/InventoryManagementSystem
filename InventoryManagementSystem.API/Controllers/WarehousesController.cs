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
    public class WarehousesController : ControllerBase
    {
        private readonly IWarehouseService _warehouseService;
        private readonly ILogger<WarehousesController> _logger;

        public WarehousesController(
            IWarehouseService warehouseService,
            ILogger<WarehousesController> logger)
        {
            _warehouseService = warehouseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetAllWarehouses()
        {
            var warehouses = await _warehouseService.GetAllWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetActiveWarehouses()
        {
            var warehouses = await _warehouseService.GetActiveWarehousesAsync();
            return Ok(warehouses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseDto>> GetWarehouseById(Guid id)
        {
            var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
            if (warehouse == null)
                return NotFound();

            return Ok(warehouse);
        }

        [HttpGet("{id}/products")]
        public async Task<ActionResult<WarehouseDto>> GetWarehouseWithProducts(Guid id)
        {
            try
            {
                var warehouse = await _warehouseService.GetWarehouseWithProductsAsync(id);
                return Ok(warehouse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WarehouseDto>> CreateWarehouse([FromBody] WarehouseCreateDto warehouseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var warehouse = await _warehouseService.CreateWarehouseAsync(warehouseDto, userId);
            
            _logger.LogInformation($"Warehouse created: {warehouse.Id} - {warehouse.Name} by user {userId}");
            return CreatedAtAction(nameof(GetWarehouseById), new { id = warehouse.Id }, warehouse);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<WarehouseDto>> UpdateWarehouse(Guid id, [FromBody] WarehouseUpdateDto warehouseDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var warehouse = await _warehouseService.UpdateWarehouseAsync(id, warehouseDto, userId);
                _logger.LogInformation($"Warehouse updated: {id} - {warehouse.Name} by user {userId}");
                return Ok(warehouse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteWarehouse(Guid id)
        {
            try
            {
                await _warehouseService.DeleteWarehouseAsync(id);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation($"Warehouse deleted: {id} by user {userId}");
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
