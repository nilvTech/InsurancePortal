
namespace InsureFlow.Domain.Common;

public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; protected set; }

    public DateTime? UpdatedAtUtc { get; protected set; }

    public long? CreatedByUserId { get; protected set; }

    public long? UpdatedByUserId { get; protected set; }

    protected AuditableEntity()
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    protected AuditableEntity(Guid externalId)
        : base(externalId)
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void SetCreatedBy(long? userId)
    {
        CreatedByUserId = userId;
    }

    public void SetUpdatedBy(long? userId)
    {
        UpdatedByUserId = userId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}

