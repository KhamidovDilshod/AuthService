using System.ComponentModel;
using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class PasswordChanged:IEvent
{
    public string Message { get; set; }
    public Guid UserId { get; }
    [JsonConstructor]
    public PasswordChanged(Guid userId, string message)
    {
        UserId = userId;
        Message = message;
    }
}