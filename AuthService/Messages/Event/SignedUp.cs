using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class SignedUp : IEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string Role { get; }

    [JsonConstructor]
    public SignedUp(Guid userId, string email, string role)
    {
        UserId = userId;
        Email = email;
        Role = role;
    }
}