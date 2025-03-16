namespace api.Domain.Entities;
public record Token(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    long ObtainedAt
);
