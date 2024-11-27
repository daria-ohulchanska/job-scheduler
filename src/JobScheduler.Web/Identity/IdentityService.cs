using JobScheduler.Core.Authentication;
using JobScheduler.Core.Identity;
using JobScheduler.Core.Services;
using JobScheduler.Shared.Configurations;
using JobScheduler.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace JobScheduler.Web.Identity;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly AuthenticationSettings _authentication;

    public IdentityService(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        ITokenService tokenService,
        IOptions<AuthenticationSettings> options)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _authentication = options.Value;
    }

    public async Task<SignInResponse?> SignInAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(password) ||
            string.IsNullOrWhiteSpace(email))
        {
            throw new Exception("Invalid credentials");
        }
        
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
        {
            throw new Exception("User not found");
        }
        
        var result = await _signInManager.PasswordSignInAsync(user, password, true, false);
        if (!result.Succeeded)
        {
            //Handle invalid PasswordSignIn
        }
        
        var roles = await _userManager.GetRolesAsync(user);
        var tokens = _tokenService.GenerateTokens(user.Id, email, roles);
        
        await _userManager.SetAuthenticationTokenAsync(user,
            _authentication.Issuer,
            _authentication.TokenName,
            tokens.RefreshToken);
        
        return new SignInResponse
        {
            Tokens = tokens,
            UserName = user.UserName
        };
    }

    public async Task SignUpAsync(string userName, string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is not null)
        {
            throw new Exception("Email already exists");
        }

        var newUser = new IdentityUser
        {
            UserName = userName,
            Email = email
        };

        var result = await _userManager.CreateAsync(newUser, password);
        if (!result.Succeeded)
        {
            throw new Exception("User creation failed");
        }
        
        await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
    }

    public async Task SignOutAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new Exception("User not found");
        }

        await _signInManager.SignOutAsync();
        await _userManager.RemoveAuthenticationTokenAsync(
            user, 
            _authentication.Issuer, 
            _authentication.TokenName);
    }
}