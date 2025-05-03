using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Repository.Interfaces
{
    public interface IAuditLogRepository : IRepositoryBase<AuditLog>
    {
        Task<IEnumerable<AuditLog>> GetAuditLogsByEntityIdAsync(Guid entityId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByEntityTypeAsync(string entityType);
        Task<IEnumerable<AuditLog>> GetAuditLogsByUserIdAsync(string userId);
        Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
