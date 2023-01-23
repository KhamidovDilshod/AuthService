using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Commands;

public class RevokeAccessToken : ICommand
{
    public Guid UserId { get; }
    public string Token { get; }

    [JsonConstructor]
    public RevokeAccessToken(Guid userId, string token)
    {
        Token = token;
        UserId = userId;
    }
}