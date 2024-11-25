using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using JobScheduler.Core.Authentication;
using JobScheduler.Shared.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace JobScheduler.Core.Services;

public class TokenService : ITokenService
{
     private AuthenticationSettings _authentication;
    
    public TokenService(IOptions<AuthenticationSettings> options)
    {
        _authentication = options.Value;
    }

    public AuthenticationTokens GenerateTokens(string userId, string email, IList<string> roles)
    {
        var tokens = new AuthenticationTokens()
        {
            AccessToken = GenerateToken(userId, email, roles),
            RefreshToken = GenerateRefreshToken()
        };

        return tokens;
    }
    
    public List<string> ValidateToken(string token)
    {
        var parameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidIssuer = _authentication.Issuer,
            ValidateAudience = true,
            ValidAudience = _authentication.Audience,
            IssuerSigningKey = _authentication.GetKey()
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.ValidateToken(token, parameters, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken 
            || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256)
            || jwtToken is null)
        {
            throw new Exception("Invalid token");
        }

        var roles = new List<string>();
        
        foreach (var claim in jwtToken.Claims)
        {
            if (claim.Type.Equals("role", StringComparison.CurrentCultureIgnoreCase))
            {
                roles.Add(claim.Value);
            }
        }
        
        return roles;
    }
    
    private string GenerateToken(string userId, string email, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new("Id", userId),
            new("Email", email),
        };

        claims.AddRange(roles.Select(x => new Claim("role", x)));

        var claimsIdentity = new ClaimsIdentity(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        const string algorithm = SecurityAlgorithms.HmacSha256;
        var credentials = new SigningCredentials(_authentication.GetKey(), algorithm);

        var token = new JwtSecurityToken(
            _authentication.Issuer,
            _authentication.Audience,
            claimsIdentity.Claims,
            notBefore: DateTime.Now,
            expires:DateTime.Now.AddSeconds(_authentication.TokenLifeTime),
            credentials);

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);
        return encodedToken;    
    }
    
    private string GenerateRefreshToken()
    {
        var bytes = new byte[_authentication.RefreshTokenSize];

        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(bytes);

        return Convert.ToBase64String(bytes);
    }
}