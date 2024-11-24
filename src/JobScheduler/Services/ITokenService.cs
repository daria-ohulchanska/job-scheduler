using JobScheduler.Core.Authentication;

namespace JobScheduler.Core.Services;

public interface ITokenService
{
    AuthenticationTokens GenerateTokens(string userId, string email, IList<string> roles);
    List<string> ValidateToken(string token);
}