using AuthService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Common.Context;

public class AuthContext:DbContext
{
    public AuthContext(DbContextOptions<AuthContext> options):base(options)
    {
        
    }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<User> Users { get; set; }
}