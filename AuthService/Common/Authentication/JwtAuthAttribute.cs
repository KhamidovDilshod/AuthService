using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuthService.Common.Authentication;

public class JwtAuthAttribute : AuthAttribute
{
    public JwtAuthAttribute(string policy = "") : base(JwtBearerDefaults.AuthenticationScheme, policy)
    {
    }
}