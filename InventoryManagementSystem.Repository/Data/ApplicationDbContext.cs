using InventoryManagementSystem.Domain.Common;
using InventoryManagementSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace InventoryManagementSystem.Repository.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TransactionArchive> TransactionArchives { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations from the current assembly
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure relationships
            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(t => t.SourceWarehouse)
                .WithMany(w => w.SourceTransactions)
                .HasForeignKey(t => t.SourceWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryTransaction>()
                .HasOne(t => t.DestinationWarehouse)
                .WithMany(w => w.DestinationTransactions)
                .HasForeignKey(t => t.DestinationWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Add global query filter for soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IBaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "p");
                    var propertyMethodInfo = typeof(EF).GetMethod(nameof(EF.Property))!.MakeGenericMethod(typeof(bool));
                    var isDeletedProperty = Expression.Call(propertyMethodInfo, parameter, Expression.Constant("IsDeleted"));
                    var compareExpression = Expression.Equal(isDeletedProperty, Expression.Constant(false));
                    var lambda = Expression.Lambda(compareExpression, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Audit trail & update timestamps
            var entries = ChangeTracker.Entries<IBaseEntity>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = DateTime.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
