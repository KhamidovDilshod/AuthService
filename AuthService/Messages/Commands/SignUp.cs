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
    public SignUp(string role, string password, string email, Guid id)
    {
        Role = role;
        Password = password;
        Email = email;
        Id = id;
    }
}