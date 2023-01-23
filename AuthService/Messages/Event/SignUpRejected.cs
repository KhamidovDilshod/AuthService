using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class SignUpRejected : IRejectedEvent
{
    public Guid UserId { get; }
    public string Reason { get; }
    public string Code { get; }
    
    [JsonConstructor]
    public SignUpRejected(Guid userId,string reason, string code)
    {
        UserId = userId;
        Reason = reason;
        Code = code;
    }
}