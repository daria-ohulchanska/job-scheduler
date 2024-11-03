using JobScheduler.Core.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduler.Web.Controllers
{
    public class AuthenticationController : Controller
    {

        public AuthenticationController()
        {
            
        }

        [HttpPost("sign-in")]
        public async Task<SignInResponse> SignInAsync(SignInRequest request)
        {
        }

        [HttpPost("sign-up")]
        public async Task SignUpAsync(SignUpRequest model)
        {
        }
    }
}
