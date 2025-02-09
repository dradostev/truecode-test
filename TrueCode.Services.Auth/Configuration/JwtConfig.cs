namespace TrueCode.Services.Auth.Configuration;

public class JwtConfig
{
    public int AccessTokenExpiresMins { get; set; }
    public int RefreshTokenExpiresDays { get; set; }
    public required string SecretKey { get; set; }
}