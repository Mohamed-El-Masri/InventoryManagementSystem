namespace InventoryManagementSystem.Domain.Common
{
    public interface IBaseEntity
    {
        Guid Id { get; set; }
        DateTime CreatedAt { get; set; }
        string? CreatedBy { get; set; }
        DateTime? ModifiedAt { get; set; }
        string? ModifiedBy { get; set; }
        bool IsDeleted { get; set; }
    }
}
