using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using webStudioBlazor.Data;

namespace webStudioBlazor.Controllers
{
    [Route("auth")]
    public class AuthController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
               
        [HttpGet("login-google")]
        public IActionResult LoginGoogle(string? returnUrl = "/")
        {          
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", new { returnUrl });
                       
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                GoogleDefaults.AuthenticationScheme,
                redirectUrl);
                       
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        
        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = "/")
        {            
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {               
                return Redirect("/login");
            }
         
            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: true);

            if (signInResult.Succeeded)
            {
                await _signInManager.UpdateExternalAuthenticationTokensAsync(info);
                return LocalRedirect(returnUrl ?? "/");
            }
                       
            var email =
                info.Principal.FindFirstValue(ClaimTypes.Email) ??
                info.Principal.FindFirstValue("email");

            if (string.IsNullOrWhiteSpace(email))
            {               
                return Redirect("/login");
            }
           
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {                
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true 
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {                    
                    return Redirect("/login");
                }
            }
                        
            var addLoginResult = await _userManager.AddLoginAsync(user, info);
            if (!addLoginResult.Succeeded)
            {               
                return Redirect("/login");
            }
                        
            await _signInManager.SignInAsync(user, isPersistent: true);
            await _signInManager.UpdateExternalAuthenticationTokensAsync(info);

            return LocalRedirect(returnUrl ?? "/");
        }
                
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
