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
        public readonly PatientContext _patientContext;
        public readonly StaffContext _staffContext;
        public AuthenticationController(ApplicationUserContext applicationUserContext,
                                        PatientContext patientContext,
                                        StaffContext staffContext)
        {
            _applicationUserContext = applicationUserContext;
            _patientContext = patientContext;
            _staffContext = staffContext;
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
                ApplicationUser? user = null;
                try
                {
                    user = await _applicationUserContext.GetUserByEmailAsync(model.Email);
                }
                catch (InvalidDataException ex)
                {
                    Console.WriteLine(ex.Message);
                    return NotFound();
                }

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
            List<string> roles = new List<string> { "Admin", "Doctor", "SupportStaff", "Patient" };
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

        [HttpGet]
        public IActionResult CreateApplicationUserAccount (int? id, bool patientFlag)
        {
            if (id == null)
            {
                return NotFound();
            }
            else
            {
                RegisterModel model = new RegisterModel();
                if (patientFlag == true)
                {
                    var patient = PatientContext.PatientList
                        .FirstOrDefault(m => m.Id == id);
                    if (patient == null)
                    {
                        return NotFound();
                    }

                    if (_applicationUserContext.UserRegisted((int)id, patientFlag).Result)
                    {
                        Console.WriteLine("This patient already has an application account!");
                        return RedirectToAction(nameof(Index));
                    }

                    model.UserName = patient.Name;
                    model.DataId = patient.Id;
                    model.Role = "Patient";
                }
                else
                {
                    var staff = StaffContext.StaffList
                        .FirstOrDefault(staff => staff.Id == id);
                    if (staff == null)
                    {
                        return NotFound();
                    }

                    if (_applicationUserContext.UserRegisted((int)id, patientFlag).Result)
                    {
                        Console.WriteLine("This staff already has an application account!");
                        return RedirectToAction(nameof(Index));
                    }

                    model.UserName = staff.Name;
                    model.DataId = staff.Id;
                    model.Role = staff.HealthCareStaff.ToString();
                }

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateApplicationUserAccount (RegisterModel model)
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

                if (user.Role == "Patient")
                {
                    return RedirectToAction(nameof(Index), "Patient");
                }
                else
                {
                    return RedirectToAction(nameof(Index), "Staff");
                }
            }

            return View();
        }

        public async Task<IActionResult> ChangePassword(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var docQuery = await _applicationUserContext.GetDocumentReferenceWithId(id).GetSnapshotAsync();
            if (docQuery == null)
            {
                return NotFound();
            }
            else
            {
                var user = docQuery.ConvertTo<ApplicationUser>();

                RegisterModel model = new RegisterModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = user.Role,
                    DataId = user.DataId
                };
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(RegisterModel model)
        {
            if (model == null || model.Password == null)
            {
                return NotFound();
            } 
            if (ModelState.IsValid)
            {
                try
                {
                    await _applicationUserContext.ChancePassWordAsync(model.Id, model.Password);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return RedirectToAction(nameof(Index));
                }
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
