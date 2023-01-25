using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Commands;

public class RevokeRefreshToken:ICommand
{
    public string Message { get; set; }
    public Guid UserId { get; }
    public string Token { get; }
    
    [JsonConstructor]
    public RevokeRefreshToken(Guid userId, string token, string message)
    {
        UserId = userId;
        Token = token;
        Message = message;
    }
}