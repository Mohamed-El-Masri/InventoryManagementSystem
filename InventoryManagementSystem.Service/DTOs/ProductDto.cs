using InventoryManagementSystem.Domain.Enums;

namespace InventoryManagementSystem.Service.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string? SKU { get; set; }
        public string? Barcode { get; set; }
        public ProductCategory Category { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Weight { get; set; }
        public string? Dimensions { get; set; }
        public Guid? WarehouseId { get; set; }
        public string? WarehouseName { get; set; }
    }

    public class ProductCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int LowStockThreshold { get; set; }
        public string? SKU { get; set; }
        public string? Barcode { get; set; }
        public ProductCategory Category { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Weight { get; set; }
        public string? Dimensions { get; set; }
        public Guid? WarehouseId { get; set; }
    }

    public class ProductUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int LowStockThreshold { get; set; }
        public string? SKU { get; set; }
        public string? Barcode { get; set; }
        public ProductCategory Category { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Weight { get; set; }
        public string? Dimensions { get; set; }
        public Guid? WarehouseId { get; set; }
    }
}
