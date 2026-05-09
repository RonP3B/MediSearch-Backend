namespace MediSearch.Core.Domain.SharedKernel.Bases;

public abstract class BaseAuditableEntity : BaseEntity, IAuditableEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "SYSTEM";
    public DateTimeOffset LastModifiedAt { get; set; }
    public string LastModifiedBy { get; set; } = "SYSTEM";
}
