using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Repositories
{
    public class NotificationRepository : RepositoryBase<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync()
        {
            return await _dbContext.Notifications
                .Include(n => n.Product)
                .Where(n => !n.IsRead && !n.IsDeleted)
                .OrderByDescending(n => n.NotificationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByUserIdAsync(string userId)
        {
            return await _dbContext.Notifications
                .Include(n => n.Product)
                .Where(n => n.UserId == userId && !n.IsDeleted)
                .OrderByDescending(n => n.NotificationDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetNotificationsByProductIdAsync(Guid productId)
        {
            return await _dbContext.Notifications
                .Include(n => n.Product)
                .Where(n => n.ProductId == productId && !n.IsDeleted)
                .OrderByDescending(n => n.NotificationDate)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _dbContext.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _dbContext.Notifications.Update(notification);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead && !n.IsDeleted)
                .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
