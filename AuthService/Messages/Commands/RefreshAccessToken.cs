using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Commands;

public class RefreshAccessToken : ICommand
{
    public string Token { get; }

    [JsonConstructor]
    public RefreshAccessToken(string token)
    {
        Token = token;
    }
}