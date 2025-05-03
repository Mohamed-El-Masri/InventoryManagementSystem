namespace InventoryManagementSystem.Service.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime NotificationDate { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
