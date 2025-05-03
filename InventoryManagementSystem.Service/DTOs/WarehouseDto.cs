namespace InventoryManagementSystem.Service.DTOs
{
    public class WarehouseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
        public int ProductCount { get; set; }
    }

    public class WarehouseCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class WarehouseUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? Code { get; set; }
        public bool IsActive { get; set; }
    }
}
