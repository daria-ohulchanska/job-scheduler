using JobScheduler.Core.Authentication;
using JobScheduler.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Web.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;

    public AccountController(
        UserManager<UserEntity> userManager, 
        SignInManager<UserEntity> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult SignUp() => 
        View();

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpRequest request)
    {
        if (ModelState.IsValid)
        {
            var user = new UserEntity
            {
                UserName = request.Email, 
                Email = request.Email, 
                Name = request.Name, 
                Surname = request.Surname
            };
            
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(request);
    }

    [HttpGet]
    public IActionResult SignIn() => View();

    [HttpPost]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(
                request.Email, 
                request.Password, 
                request.RememberMe, 
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }

        return View(request);
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}