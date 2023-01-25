using System.Text.Json.Serialization;
using AuthService.Common.Messages;

namespace AuthService.Messages.Commands;

public class SignUp : ICommand
{
    public Guid Id { get; }
    public string Email { get; }
    public string Password { get; }
    public string Role { get; }
    
    [JsonConstructor]
    public SignUp(string role, string password, string email, Guid id, Guid userId, string message)
    {
        Role = role;
        Password = password;
        Email = email;
        Id = id;
        UserId = userId;
        Message = message;
    }

    public string Message { get; set; }
    public Guid UserId { get; }
}