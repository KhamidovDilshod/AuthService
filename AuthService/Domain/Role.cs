namespace AuthService.Domain;

public static class Role
{
    public const string User = "user";
    public const string Admin = "Admin";

    public static bool IsValid(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return false;
        }

        role = role.ToLowerInvariant();

        return role is User or Admin;
    }
}