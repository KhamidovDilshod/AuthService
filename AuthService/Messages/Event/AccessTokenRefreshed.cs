using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class AccessTokenRefreshed:IEvent
{
    public Guid UserId { get; }
    
    [JsonConstructor]
    public AccessTokenRefreshed(Guid userId)
    {
        UserId = userId;
    }
}