using InventoryManagementSystem.Domain.Common;
using System.Linq.Expressions;

namespace InventoryManagementSystem.Repository.Interfaces
{
    public interface IRepositoryBase<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null,
            bool disableTracking = true);

        Task<T?> GetByIdAsync(Guid id);
        
        Task<T> AddAsync(T entity);
        
        Task UpdateAsync(T entity);
        
        Task DeleteAsync(T entity);
        
        Task HardDeleteAsync(T entity);
        
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
