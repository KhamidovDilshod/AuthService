using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class RefreshTokenRevoked : IEvent
{
    public Guid UserId { get; }

    [JsonConstructor]
    public RefreshTokenRevoked(Guid userId)
    {
        UserId = userId;
    }
}