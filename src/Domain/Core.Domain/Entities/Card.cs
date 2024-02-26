using Core.Domain.Common;

namespace Core.Domain.Entities;

public class Card : IAuditableEntityWithActor
{
    public long CardId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }   
    public string? Color { get; set; }
    public int CardStatusId { get; set; }
    public long CreatedById { get; set; }
    public long? ModifiedById { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public CardStatus CardStatus { get; set; } = null!;
    public User CreatedBy { get; set; } = null!;
    public User? ModifiedBy { get; set; }    
}
