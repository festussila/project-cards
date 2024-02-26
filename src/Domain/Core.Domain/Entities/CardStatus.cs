using Core.Domain.Common;

namespace Core.Domain.Entities;

public class CardStatus : IAuditableEntity
{
    public int CardStatusId { get; set; }
    public string Status { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }

    public ICollection<Card> Cards { get; set; } = new List<Card>();
}
