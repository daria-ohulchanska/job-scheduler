using JobScheduler.Core.Authentication;
using JobScheduler.Core.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Web.Controllers;

public class AccountController : Controller
{
    private readonly IIdentityService _identityService;

    public AccountController(IIdentityService identityService)
    {
        _identityService = identityService;
    }
    
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        await _identityService.SignUpAsync(request.UserName, request.Email, request.Password);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        await _identityService.SignInAsync(request.Email, request.Password);
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> SignOut(string email)
    {
        await _identityService.SignOutAsync(email);
        return RedirectToAction("Index", "Home");
    }
}