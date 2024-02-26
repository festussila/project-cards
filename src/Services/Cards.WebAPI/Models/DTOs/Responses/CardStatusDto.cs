namespace Cards.WebAPI.Models.DTOs.Responses;

public class CardStatusDto
{
    /// <summary>
    /// Card status unique identifier
    /// </summary>
    public string? CardStatusId { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public string? Status { get; set; }
}