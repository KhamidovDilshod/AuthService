using AuthService.Common.Authentication;
using AuthService.Domain;

namespace AuthService.Services;

public interface IIdentityService
{
    Task SingUpAsync(Guid id, string email, string password, string role = Role.User);
    Task<JsonWebToken> SIgnInAsync(string email, string password);
    Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
}