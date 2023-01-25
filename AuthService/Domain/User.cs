using System.Text.RegularExpressions;
using AuthService.Common.Types;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Domain;

public class User : IIdentifiable
{
    private static readonly Regex EmailRegex = new Regex(
        @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
        @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");

    private Guid _id;
    public Guid Id
    {
        get => _id;
        private set => _id = value;
    }

    public string Email { get; private set; }
    public string Role { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User()
    {
    }

    public User(Guid id, string email, string role)
    {
        if (!EmailRegex.IsMatch(email))
        {
            throw new AuthException(Codes.InvalidEmail, $"Invalid email: '{email}'.");
        }

        if (!Domain.Role.IsValid(role))
        {
            throw new AuthException(Codes.InvalidRole, $"Invalid role :'{role}.'");
        }

        Id = id;
        Email = email.ToLowerInvariant();
        Role = role.ToLowerInvariant();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPassword(string password, IPasswordHasher<User> passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new AuthException($"Password can not be empty.");
        }

        PasswordHash = passwordHasher.HashPassword(this, password);
    }

    public bool ValidatePassword(string password, IPasswordHasher<User> passwordHasher)
        => passwordHasher.VerifyHashedPassword(this, PasswordHash, password) == PasswordVerificationResult.Failed;
}