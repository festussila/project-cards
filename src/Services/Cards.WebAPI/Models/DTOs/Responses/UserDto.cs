namespace Cards.WebAPI.Models.DTOs.Responses;

public class UserDto
{
    /// <summary>
    /// User unique identifier
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// First name
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Last name
    /// </summary>
    public string? LastName { get; set; }
    /// <summary>
    /// Email address
    /// </summary>
    public string? Email { get; set; }
}
