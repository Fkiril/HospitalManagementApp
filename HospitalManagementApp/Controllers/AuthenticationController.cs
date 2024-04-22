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
            [Bind("Email,Password")]LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _applicationUserContext.GetUserByEmailAsync(model.Email);

                if (user != null)
                {
                    if (user.Password == model.Password)
                    {
                        var identity = new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Email, user.Email),
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                new Claim(ClaimTypes.Role, user.Role),
                                new Claim(ClaimTypes.UserData, user.DataId.ToString())
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
                        ModelState.AddModelError("", "Wrong password!");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email!");
                }

            }

            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            List<string> roles = new List<string> { "Admin", "Doctor", "Patient" };
            ViewBag.Roles = roles;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            [Bind("Id,UserName,Email,Password,ConfirmPassword,Role,DataId")]RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    Id = model.Id,
                    UserName = model.UserName,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role,
                    DataId = model.DataId
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
