using AutoMapper;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Repository.UnitOfWork;
using InventoryManagementSystem.Service.DTOs;

namespace InventoryManagementSystem.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<NotificationDto> CreateLowStockNotificationAsync(Product product)
        {
            var notification = new Notification
            {
                Title = "Low Stock Alert",
                Message = $"Product '{product.Name}' is below the threshold. Current quantity: {product.Quantity}, Threshold: {product.LowStockThreshold}",
                ProductId = product.Id,
                IsRead = false,
                NotificationDate = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<NotificationDto> CreateNotificationAsync(string title, string message, Guid? productId = null, string? userId = null)
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                ProductId = productId,
                UserId = userId,
                IsRead = false,
                NotificationDate = DateTime.UtcNow
            };

            await _unitOfWork.Notifications.AddAsync(notification);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<NotificationDto>(notification);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync()
        {
            var notifications = await _unitOfWork.Notifications.GetUnreadNotificationsAsync();
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(string userId)
        {
            var notifications = await _unitOfWork.Notifications.GetNotificationsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            await _unitOfWork.Notifications.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            await _unitOfWork.Notifications.MarkAllAsReadAsync(userId);
        }
    }
}
