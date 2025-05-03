using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Repository.Interfaces
{
    public interface INotificationRepository : IRepositoryBase<Notification>
    {
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync();
        Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId);
        Task<IEnumerable<Notification>> GetNotificationsByProductIdAsync(Guid productId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(string userId);
    }
}
