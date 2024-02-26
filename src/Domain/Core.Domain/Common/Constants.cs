namespace Core.Domain.Common;

public class Constants
{

    public const string AccessToken = "access_token";
    public const string ClaimsSub = "sub";
    public const string JwtKey = "JWT:Key";
    public const string JwtIssuer = "JWT:Issuer";
    public const string JwtAudience = "JWT:Audience";
    public const string JwtLifetime = "JWT:LifetimeInMins";
    public const string CorsPolicy = "AllowAllOrigins";

    public const int MinDefaultLength = 0;
    public const int MaxDescription = 150;

    public const int MaxCardName = 50;
    public const int MinCardName = 5;
    public const int MaxCardHexColor = 7;
    public const int MaxCardStatus = 15;

    public const int MaxUserEmail = 150;
    public const int MaxUserName = 50;
    public const int MaxRoleName = 15;
    public const int MaxMsisdn = 20;
    public const int MaxPassword = 256;
    public const int MinPassword = 8;
    public const int MinEmail = 3;

    public const int DefaultPage = 1;
    public const int DefaultPageSize = 10;
}