using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Commands;

public class SignIn:ICommand
{
    public string Email { get; }
    public string Password { get; }
    
    [JsonConstructor]
    public SignIn(string email, string password, string message, Guid userId)
    {
        Email = email;
        Password = password;
        Message = message;
        UserId = userId;
    }

    public string Message { get; set; }
    public Guid UserId { get; }
}