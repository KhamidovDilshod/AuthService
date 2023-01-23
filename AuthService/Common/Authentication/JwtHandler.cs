﻿using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.Common.Types;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace AuthService.Common.Authentication;

public class JwtHandler : IJwtHandler
{
    private static readonly ISet<string> DefaultClaims = new HashSet<string>
    {
        JwtRegisteredClaimNames.Sub,
        JwtRegisteredClaimNames.UniqueName,
        JwtRegisteredClaimNames.Jti,
        JwtRegisteredClaimNames.Iat,
        ClaimTypes.Role
    };

    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
    private readonly JwtOptions _options;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public JwtHandler(JwtOptions options)
    {
        _options = options;
        var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));
        _signingCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256);
        _tokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = issuerSigningKey,
            ValidIssuer = _options.Issuer,
            ValidateAudience = _options.ValidateAudience,
            ValidAudience = _options.ValidAudience,
            ValidateLifetime = _options.ValidateLifetime
        };
    }


    public JsonWebToken CreateToken(string userId, string role = null, IDictionary<string, string> claims = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new AuthException("User id claim can not be empty.", nameof(userId));
        }

        var now = DateTime.UtcNow;
        var jwtClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToString(CultureInfo.InvariantCulture)),
        };
        if (!string.IsNullOrWhiteSpace(role))
        {
            jwtClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var customClaims = claims?.Select(claim => new Claim(claim.Key, claim.Value)).ToArray()
                           ?? Array.Empty<Claim>();
        jwtClaims.AddRange(customClaims);
        var expires = now.AddMinutes(_options.ExpiryMinutes);
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            claims: jwtClaims,
            notBefore: now,
            expires: expires,
            signingCredentials: _signingCredentials
        );
        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new JsonWebToken
        {
            AccessToken = token,
            RefreshToken = string.Empty,
            Expires = expires.ToFileTimeUtc(),
            Id = userId,
            Role = role ?? string.Empty,
            Claims = customClaims.ToDictionary(c => c.Type, c => c.Value)
        };
    }

    public JsonWebTokenPayload GetTokenPayload(string accessToken)
    {
        _jwtSecurityTokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var validatedToken);
        if (!(validatedToken is JwtSecurityToken jwt))
        {
            return null!;
        }

        return new JsonWebTokenPayload
        {
            Subject = jwt.Subject,
            Role = jwt.Claims.SingleOrDefault(x => x.Type == ClaimTypes.Role)?.Value!,
            Expires = jwt.ValidTo.ToFileTimeUtc(),
            Claims = jwt.Claims.Where(x => !DefaultClaims.Contains(x.Type)).ToDictionary(k => k.Type, v => v.Value)
        };
    }
}