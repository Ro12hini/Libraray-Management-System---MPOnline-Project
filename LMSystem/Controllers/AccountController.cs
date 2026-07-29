using LMSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMSystem.Controllers
{
    // Minimal custom login/logout screens built directly on top of
    // SignInManager/UserManager (i.e. AddIdentity, not the full Identity UI scaffolding).
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError(string.Empty, "Email and password are required.");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                    ? Redirect(returnUrl)
                    : RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "This account has been locked out. Try again later.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
