using Microsoft.AspNetCore.Mvc;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using System.Security.Cryptography;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin, Doctor, Nurse, SupportStaff", AuthenticationSchemes = "Cookies")]
    public class StaffController : Controller
    {
        public readonly PatientContext _patientContext;
        public readonly StaffContext _context;
        public readonly IHttpContextAccessor _httpContextAccessor;
        public StaffController(PatientContext patientContext,
                                StaffContext staffContext,
                                IHttpContextAccessor httpContextAccessor)
        {
            _patientContext = patientContext;
            _context = staffContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [Authorize(Roles = "Admin, Doctor, Nurse, SupportStaff", AuthenticationSchemes = "Cookies")]
        // GET: Staff
        public async Task<IActionResult> Index()
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.IsInRole("Admin"))
            {
                await _context.InitializeStaffListFromFirestore();
                return View(StaffContext.StaffList);
            }
            else
            {
                int sId;
                var userDataClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData);
                if (userDataClaim != null)
                {
                    sId = Convert.ToInt32(userDataClaim.Value);
                    var staff = _context.GetStaffWithId(sId);

                    if (staff != null)
                    {
                        return RedirectToAction(nameof(Details), new { id = sId });
                    }
                }
            }

            if (TempData["ErrorMessage"] != null && TempData["ErrorMessage"] as string != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            }

            if (TempData["StaffList"] == null)
            {
                await _context.InitializeStaffListFromFirestore();
                return View(StaffContext.StaffList);
            }
            else
            {
                var ids = TempData["StaffIdList"] as List<int>;
                ICollection<Staff> staffList = _context.GetStaffListFromIdList(ids);
                if (staffList != null)
                {
                    return View(staffList);
                }
                return View(null);
            }

        }

        [Authorize(Roles = "Admin, Doctor,Nurse, SupportStaff", AuthenticationSchemes = "Cookies")]
        // GET: Staff/Details/3
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = StaffContext.StaffList.FirstOrDefault(staff => staff.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        // GET: Staff/Add
        public IActionResult Add()
        {
            return View();
        }
        //POST: Staff/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        //GET: Staff/Edit/3
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = StaffContext.StaffList
                .FirstOrDefault(s => s.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }
        //POST: Staff/Edit/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!StaffExists((int)staff.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var modelStateVal = ModelState[modelStateKey];
                    if (modelStateVal != null) foreach (var error in modelStateVal.Errors)
                        {
                            var errorMessage = error.ErrorMessage;
                            var exception = error.Exception;
                            throw new Exception(errorMessage, exception);
                        }
                }
            }
            return View(staff);
        }

        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        //GET: Staff/Remove/3
        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = StaffContext.StaffList
                .FirstOrDefault(s => s.Id == id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }
        //POST: Staff/Remove/3
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var staff = StaffContext.StaffList
                .FirstOrDefault(staff => staff.Id == id);
            if (staff != null)
            {
                StaffContext.StaffList.Remove(staff);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        public IActionResult RemoveCalendar()
        {
            if (StaffContext.StaffList == null)
            {
                return Error("List staff is empty");
            }

            else
            {
                return View();
            }
            
        }

        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        [HttpPost, ActionName("RemoveCalendar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCalendarConfirmed()
        {
            foreach(var staff in StaffContext.StaffList)
            {
                if(staff.WorkSchedule != null)
                {
                    staff.WorkSchedule = null;

                    staff.changed = true;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error(string error)
        {
            try
            {
                throw new Exception(error);
            }
            catch (Exception ex)
            {
                return View("Error", ex);
            }
        }

        [Authorize(Roles = "Admin, Doctor,Nurse", AuthenticationSchemes = "Cookies")]
        public IActionResult ShowPatient(int id)
        {
            var staff = StaffContext.StaffList
                  .FirstOrDefault(staff => staff.Id == id);
            if (staff != null)
            {
                return RedirectToAction("ShowPatientList", "Patient", new { staffId = id });
            }

            return NotFound();
        }

        private IActionResult ArgumentException(string v)
        {
            throw new NotImplementedException();
        }

        private bool StaffExists(int id)
        {
            return StaffContext.StaffList.Any(staff => staff.Id == id);
        }

        //GET: Staff/Calendar/3
        public IActionResult Calendar(int id)
        {
            var staff = StaffContext.StaffList
                .FirstOrDefault(staff => staff.Id == id);
            if (staff == null)
            {
                return NotFound();
            }
            else
            {
                if (staff.WorkSchedule == null)
                    return Error("No calendar");
            }
            return View(staff);
        }

        //POST: Staff/Calendar/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calendar(int id, Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!StaffExists((int)staff.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }

        public IActionResult CreateCalendar()
        {
            if (StaffContext.StaffList == null)
            {
                return Error("List staff is empty");
            }

            else
            {
                return View();
            }

        }

        [HttpPost, ActionName("CreateCalendar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCalendarConfirm()
        {

            if (ModelState.IsValid)
            {
                _context.CreateCalendar();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        public IActionResult FindStaff(string StaffName)
        {
            if (string.IsNullOrWhiteSpace(StaffName))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var staffList = StaffContext.StaffList.Where(s => s.Name.Contains(StaffName)).ToList();
                if (staffList.Any())
                {
                    return View("Index", staffList); 
                }
                else
                {
                    return NotFound();
                }
            }
        }

    }
}
