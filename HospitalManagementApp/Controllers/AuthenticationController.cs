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
                    ViewBag.ErrorMessage = ex.Message;
                    return View();
                }

                if (user != null)
                {
                    if (user.Password == model.Password)
                    {
                        var identity = new ClaimsIdentity(
                            new[]
                            {
                                new Claim(ClaimTypes.Name, user.UserName?? String.Empty),
                                new Claim(ClaimTypes.Email, user.Email?? String.Empty),
                                new Claim(ClaimTypes.NameIdentifier, user.Id),
                                new Claim(ClaimTypes.Role, user.Role ?? String.Empty),
                                new Claim(ClaimTypes.UserData, user.DataId.ToString()?? String.Empty)
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
                        ViewBag.ErrorMessage = "Incorrect password!";
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid email!";
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

                try
                {
                    await _applicationUserContext.AddUserAsync(user);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View();
                }

                return RedirectToAction("Login", "Authentication");
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        public async Task<IActionResult> CreateApplicationUserAccount (int? id, bool patientFlag)
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
                        TempData["ErrorMessage"] = "This patient already has a registed account";
                        return RedirectToAction(nameof(Index), "Patient");
                    }

                    if (patient.Email != null)
                    {
                        ApplicationUser? user = null;
                        try
                        {
                            user = await _applicationUserContext.GetUserByEmailAsync(patient.Email);
                        }
                        catch (InvalidDataException ex)
                        {
                            
                        }
                        if (user != null)
                        {
                            TempData["ErrorMessage"] = "This email is already registed for an account!";
                            return RedirectToAction(nameof(Index), "Patient");
                        }
                    }
                    
                    model.UserName = patient.Name;
                    model.Email = patient.Email;
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
                        TempData["ErrorMessage"] = "This staff already has a registed account!";
                        return RedirectToAction(nameof(Index), "Staff");
                    }

                    model.UserName = staff.Name;
                    model.Email = staff.Email;
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

                try
                {
                    await _applicationUserContext.AddUserAsync(user);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                }

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
                    ViewBag.ErrorMessage = ex.Message;
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
