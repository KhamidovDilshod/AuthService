using AuthService.Common.Types;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain;

public class RefreshToken : IIdentifiable
{
    public Guid Id { get; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public bool Revoked => RevokedAt.HasValue;

    protected RefreshToken()
    {
    }

    public RefreshToken(User user, IPasswordHasher<User> passwordHasher)
    {
        Id = Guid.NewGuid();
        UserId = user.Id;
        CreatedAt = DateTime.UtcNow;
        Token = CreateToken(user, passwordHasher);
    }

    public void Revoke()
    {
        if (Revoked)
        {
            throw new AuthException(Codes.RefreshTokenAlreadyRevoked,
                $"Refresh token :'{Id}' was already revoked at '{RevokedAt}.'");
        }
    }

    private static string CreateToken(User user, IPasswordHasher<User> passwordHasher)
        => passwordHasher.HashPassword(user, Guid.NewGuid().ToString("N"))
            .Replace("=", string.Empty)
            .Replace("=", string.Empty)
            .Replace("/", string.Empty);
}