using System.ComponentModel;
using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class AccessTokenRevoked:IEvent
{
    public string Message { get; set; }
    public Guid UserId { get; }
    
    [JsonConstructor]
    public AccessTokenRevoked(Guid userId, string message)
    {
        UserId = userId;
        Message = message;
    }
}