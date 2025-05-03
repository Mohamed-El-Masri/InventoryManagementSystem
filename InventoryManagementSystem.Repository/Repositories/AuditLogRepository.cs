using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Repository.Data;
using InventoryManagementSystem.Repository.Interfaces;
using InventoryManagementSystem.Repository.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Repository.Repositories
{
    public class AuditLogRepository : RepositoryBase<AuditLog>, IAuditLogRepository
    {
        public AuditLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByEntityIdAsync(Guid entityId)
        {
            return await _dbContext.AuditLogs
                .Where(al => al.EntityId == entityId && !al.IsDeleted)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByEntityTypeAsync(string entityType)
        {
            return await _dbContext.AuditLogs
                .Where(al => al.EntityType == entityType && !al.IsDeleted)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByUserIdAsync(string userId)
        {
            return await _dbContext.AuditLogs
                .Where(al => al.UserId == userId && !al.IsDeleted)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetAuditLogsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbContext.AuditLogs
                .Where(al => al.Timestamp >= startDate && al.Timestamp <= endDate && !al.IsDeleted)
                .OrderByDescending(al => al.Timestamp)
                .ToListAsync();
        }
    }
}
