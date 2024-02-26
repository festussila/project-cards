namespace Cards.WebAPI.Models.DTOs.Responses;

public class CardDto
{
    /// <summary>
    /// Card unique identifier
    /// </summary>
    public string? CardId { get; set; }

    /// <summary>
    /// Name of card
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Card description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Card color
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Card status
    /// </summary>
    public CardStatusDto? CardStatus { get; set; }

    /// <summary>
    /// Card creator/owner
    /// </summary>
    public UserDto? CreatedBy { get; set; }

    /// <summary>
    /// Card modifier
    /// </summary>
    public long? ModifiedById { get; set; }

    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Modification date
    /// </summary>
    public DateTime ModifiedAt { get; set; }
}
