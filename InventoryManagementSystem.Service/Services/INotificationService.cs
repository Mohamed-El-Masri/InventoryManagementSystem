using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateLowStockNotificationAsync(Product product);
        Task<NotificationDto> CreateNotificationAsync(string title, string message, Guid? productId = null, string? userId = null);
        Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync();
        Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(string userId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
