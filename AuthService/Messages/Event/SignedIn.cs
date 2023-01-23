using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class SignedIn:IEvent
{
    public Guid UserId { get; }
    
    [JsonConstructor]
    public SignedIn(Guid userId)
    {
        UserId = userId;
    }
}