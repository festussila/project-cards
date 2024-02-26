using System.ComponentModel.DataAnnotations;

using static Core.Domain.Common.Constants;

namespace Cards.WebAPI.Models.DTOs.Requests;

public class EditCardRequest
{
    /// <summary>
    /// Identifier of card to update
    /// </summary>
    [Required(AllowEmptyStrings = false, ErrorMessage = "CardId must be provided")]
    [Range(1, long.MaxValue, ErrorMessage = "CardId must be greater than 0")]
    public string CardId { get; set; } = null!;

    /// <summary>
    /// Name on card
    /// </summary>
    [StringLength(maximumLength: MaxCardName, MinimumLength = MinCardName, ErrorMessage = "Name should be between 5-50 characters")]
    public string? Name { get; set; }

    /// <summary>
    /// Card description
    /// </summary>
    [StringLength(maximumLength: MaxDescription, ErrorMessage = "Description max length is 150 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Card color
    /// </summary>
    [RegularExpression("^#[a-fA-F0-9]{6}$", ErrorMessage = "Invalid Hex Color Code")]
    public string? Color { get; set; }

    /// <summary>
    /// Card status
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "CardStatusId must be greater than 0")]
    public int? CardStatusId { get; set; }
}