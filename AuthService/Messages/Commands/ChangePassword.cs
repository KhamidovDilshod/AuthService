using System.Text.Json.Serialization;
using AuthService.Common.Messages;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Messages.Commands;

public class ChangePassword : ICommand
{
    public string Message { get; set; }
    public Guid UserId { get; }
    public string CurrentPassword { get; }
    public string NewPassword { get; }

    [JsonConstructor]
    public ChangePassword(Guid userId, string currentPassword, string newPassword, string message)
    {
        UserId = userId;
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
        Message = message;
    }
}