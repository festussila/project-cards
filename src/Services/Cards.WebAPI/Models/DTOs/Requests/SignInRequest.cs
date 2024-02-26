using System.ComponentModel.DataAnnotations;

using static Core.Domain.Common.Constants;

namespace Cards.WebAPI.Models.DTOs.Requests;

public class SignInRequest
{
    /// <summary>
    /// User email address
    /// </summary>
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email provided")]
    [StringLength(maximumLength: MaxUserEmail, MinimumLength = 2, ErrorMessage = "Email should be between 3-150 characters")]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User password
    /// </summary>
    [Required(ErrorMessage = "Password is required")]
    [StringLength(maximumLength: MaxPassword, MinimumLength = MinPassword, ErrorMessage = "Password should be at least 8 characters")]
    public string Password { get; set; } = null!;
}
