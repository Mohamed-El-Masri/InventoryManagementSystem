using InventoryManagementSystem.Service.DTOs;
using InventoryManagementSystem.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagementSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationService notificationService,
            ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet("unread")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUnreadNotifications()
        {
            try
            {
                var notifications = await _notificationService.GetUnreadNotificationsAsync();
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unread notifications");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var notifications = await _notificationService.GetNotificationsByUserIdAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user notifications");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            try
            {
                await _notificationService.MarkAsReadAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification as read");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }

        [HttpPut("read-all")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _notificationService.MarkAllAsReadAsync(userId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read");
                return StatusCode(500, "Internal server error occurred while processing your request");
            }
        }
    }
}
