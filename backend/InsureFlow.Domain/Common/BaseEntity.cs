namespace InsureFlow.Domain.Common;

public abstract class BaseEntity
{
    public long Id { get; protected set; }

    public Guid ExternalId { get; protected set; }

    protected BaseEntity()
    {
        ExternalId = Guid.NewGuid();
    }

    protected BaseEntity(Guid externalId)
    {
        ExternalId = externalId == Guid.Empty
            ? Guid.NewGuid()
            : externalId;
    }
}

