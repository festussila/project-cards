using System.ComponentModel.DataAnnotations;

using static Core.Domain.Common.Constants;

namespace Cards.WebAPI.Models.DTOs.Requests;

public class CardRequest
{
    /// <summary>
    /// Name on card
    /// </summary>
    [Required(ErrorMessage = "Name is required")]
    [StringLength(maximumLength: MaxCardName, MinimumLength = MinCardName, ErrorMessage = "Name should be between 5-50 characters")]
    public string Name { get; set; } = null!;

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
}
