using Microsoft.AspNetCore.Mvc;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using HospitalManagementApp.Models.AuthenticationModels;
using HospitalManagementApp.Data;
using Microsoft.AspNetCore.Authorization;


namespace HospitalManagementApp.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        public readonly ApplicationUserContext _applicationUserContext;
        public AuthenticationController(ApplicationUserContext applicationUserContext)
        {
            _applicationUserContext = applicationUserContext;
        }

        // GET: Authentication
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [Bind("Email,Password,RememberMe")]LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _applicationUserContext.GetUserByEmailAsync(model.Email);

                if (user != null)
                {
                    Console.WriteLine("user's role: " + user.Role);
                    if (user.Password == model.Password)
                    {
                        var identity = new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                new Claim(ClaimTypes.Role, user.Role),
                            },
                            CookieAuthenticationDefaults.AuthenticationScheme
                            );
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                        };


                        await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(identity),
                            authProperties);

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid email or password");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Can not find that email in database");
                }

            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            [Bind("Id,UserName,Email,Password,ConfirmPassword,Role")]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Id = model.Id,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role
                };

                await _applicationUserContext.AddUserAsync(user);

                return RedirectToAction("Login", "Authentication");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }
    }
}
