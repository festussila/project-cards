namespace Core.Domain.Common;

public interface IAuditableEntityWithActor
{
    public long CreatedById { get; set; }
    public long? ModifiedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
