namespace Cards.WebAPI.Models.DTOs.Responses;

/// <param name="AccessToken"> Token to be used in the subsequent calls for authentication allowing access to the API resources </param>
/// <param name="Expires"> Timestamp at which the token expires </param>
/// <param name="User"> Logged in user </param>
public record SignInDto(string? AccessToken, long Expires, UserDto? User);
