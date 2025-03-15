namespace api.Models;
public record Token(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    long ObtainedAt
);
