using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Commands;

public class RefreshAccessToken : ICommand
{
    public string Token { get; }

    [JsonConstructor]
    public RefreshAccessToken(string token, Guid userId, string message)
    {
        Token = token;
        UserId = userId;
        Message = message;
    }

    public string Message { get; set; }
    public Guid UserId { get; }
}