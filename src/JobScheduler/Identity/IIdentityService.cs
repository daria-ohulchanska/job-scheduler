using JobScheduler.Core.Authentication;

namespace JobScheduler.Core.Identity;

public interface IIdentityService
{
    Task<SignInResponse?> SignInAsync(string email, string password);
    Task SignUpAsync(string userName, string email, string password);
    Task SignOutAsync(string email);
}