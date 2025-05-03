using InventoryManagementSystem.API.Filters;
using InventoryManagementSystem.Domain.Enums;
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
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            
            if (product == null)
                return NotFound();
                
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] ProductCreateDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var product = await _productService.CreateProductAsync(productDto, userId);
            
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] ProductUpdateDto productDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var product = await _productService.UpdateProductAsync(id, productDto, userId);
                return Ok(product);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("low-stock")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStockProducts()
        {
            var products = await _productService.GetLowStockProductsAsync();
            return Ok(products);
        }

        [HttpGet("by-category/{category}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByCategory(ProductCategory category)
        {
            var products = await _productService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        [HttpGet("by-warehouse/{warehouseId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProductsByWarehouse(Guid warehouseId)
        {
            var products = await _productService.GetProductsByWarehouseAsync(warehouseId);
            return Ok(products);
        }
    }
}
