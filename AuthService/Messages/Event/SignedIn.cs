using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class SignedIn:IEvent
{
    public string Message { get; set; }
    public Guid UserId { get; }
    
    [JsonConstructor]
    public SignedIn(Guid userId, string message)
    {
        UserId = userId;
        Message = message;
    }
}