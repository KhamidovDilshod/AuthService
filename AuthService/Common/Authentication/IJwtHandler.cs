﻿namespace AuthService.Common.Authentication;

public interface IJwtHandler
{
    JsonWebToken CreateToken(string userId, string role = null, IDictionary<string, string> claims = null);
    JsonWebTokenPayload GetTokenPayload(string accessToken);
}