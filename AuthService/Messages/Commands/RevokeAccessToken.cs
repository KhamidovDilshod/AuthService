using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Commands;

public class RevokeAccessToken : ICommand
{
    public string Message { get; set; }
    public Guid UserId { get; }
    public string Token { get; }

    [JsonConstructor]
    public RevokeAccessToken(Guid userId, string token, string message)
    {
        Token = token;
        Message = message;
        UserId = userId;
    }
}