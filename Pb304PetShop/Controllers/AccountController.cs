using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Pb304PetShop.DataContext.Entities;
using Pb304PetShop.Models;

namespace Pb304PetShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = new AppUser
            {
                UserName = registerViewModel.Username,
                Email = registerViewModel.Email,
                FullName = registerViewModel.FullName,
            };

            var result = await _userManager.CreateAsync(user, registerViewModel.Password);

            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View();
            }

            return RedirectToAction(nameof(Login));
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var existUser = await _userManager.FindByNameAsync(loginViewModel.UserName);

            if (existUser == null) 
            {
                ModelState.AddModelError("", "Username or password is incorrect");

                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(existUser, loginViewModel.Password, loginViewModel.RememberMe, true);

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", $"You are banned {existUser.LockoutEnd.Value - DateTimeOffset.UtcNow}");
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Username or password is incorrect");

                return View();
            }

            if (loginViewModel.ReturnUrl != null)
            {
                return Redirect(loginViewModel.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
