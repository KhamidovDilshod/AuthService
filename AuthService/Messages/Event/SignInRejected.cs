using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Event;

public class SignInRejected : IRejectedEvent
{
    public string Email { get; }
    public string Reason { get; }
    public string Code { get; }
    
    [JsonConstructor]
    public SignInRejected(string email, string reason, string code)
    {
        Email = email;
        Reason = reason;
        Code = code;
    }
}