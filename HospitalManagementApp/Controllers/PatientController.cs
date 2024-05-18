using Microsoft.AspNetCore.Mvc;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Google.Cloud.Firestore;
using HospitalManagementApp.Models.PatientViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ViewFeatures;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin, Doctor, Patient", AuthenticationSchemes = "Cookies")]
    public class PatientController : Controller
    {
        public readonly PatientContext _patientContext;
        public readonly StaffContext _staffContext;
        public readonly IHttpContextAccessor _httpContextAccessor;
        public PatientController(PatientContext patientContext,
                                StaffContext staffContext,
                                IHttpContextAccessor httpContextAccessor)
        {
            _patientContext = patientContext;
            _staffContext = staffContext;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: Patient
        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public async Task<IActionResult> Index()
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.User.IsInRole("Patient"))
            {
                int pId;
                var userDataClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.UserData);
                if (userDataClaim != null)
                {
                    pId = Convert.ToInt32(userDataClaim.Value);
                    var patient = _patientContext.GetPatientById(pId);

                    if (patient != null)
                    {
                        return RedirectToAction(nameof(Details), new { id = pId });
                    }
                }
            }

            if (TempData["ErrorMessage"] != null && TempData["ErrorMessage"] as string != null)
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
            }

            if (TempData["PatientList"] == null)
            {
                await _patientContext.InitializePatientListFromFirestore();
                return View(PatientContext.PatientList);
            }
            else
            {
                var ids = TempData["PatientIdList"] as List<int>;
                ICollection<Patient> patients = _patientContext.GetPatientListFromIds(ids).Result;
                if (patients != null)
                {
                    return View(patients);
                }
                return View(null);
            }
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult ShowPatientList(int id)
        {
            var patients = _patientContext.GetPatientsFromStaffId(id).Result;
            var ids = _patientContext.FromPatientListToIds(patients);

            if (ids == null || ids.Count == 0)
            {
                return NotFound();
            }
            else
            {
                TempData["PatientIdList"] = ids;

                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult SearchByDiseaseType(string searchType)
        {
            if (searchType == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                SpecialList type;
                if (Enum.TryParse(searchType, out type))
                {
                    var patientList = PatientContext.PatientList.Where(p =>
                    p.TestResult != null && p.TestResult.Type != null && p.TestResult.Type == type
                    ).ToList();

                    return View(patientList);
                }
                else
                {
                    return NotFound();
                }
            }
        }

        // GET: Patient/Details/3
        [Authorize(Roles = "Admin, Doctor, Patient", AuthenticationSchemes = "Cookies")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = PatientContext.PatientList.FirstOrDefault(patient => patient.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // GET: Patient/Add
        [Authorize(Roles = "Admin")]
        public IActionResult Add()
        {
            return View();
        }
        //POST: Patient/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add (Patient patient)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _patientContext.Add(patient);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return View();
                }
                await _patientContext.SaveChangesAsync();

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
            return View();
        }

        //GET: Patient/Edit/3
        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = PatientContext.PatientList
                .FirstOrDefault(patient => patient.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }
        //POST: Patient/Edit/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Patient patient)
        {
            if (id != patient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _patientContext.Update(patient);
                await _patientContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(patient);
        }

        //GET: Patient/Remove/3
        [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }
        //POST: Patient/Remove/3
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(patient => patient.Id == id);
            if (patient != null)
            {
                PatientContext.PatientList.Remove(patient);
            }

            await _patientContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult TreatmentScheduleManager(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult AddSchedule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            Models.Calendar? docSchedule = _staffContext.GetCalendar(patient.StaffIds);
            if (docSchedule != null)
            {
                ViewBag.Date = docSchedule.Date;
                ViewBag.DayOfWeek = docSchedule.DayofWeek;
            }

            ViewBag.PatientId = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSchedule(int patientId, TreatmentScheduleEle newTreatment)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (newTreatment == null)
                {
                    return BadRequest();
                }

                Models.Calendar? docSchedule = _staffContext.GetCalendar(patient.StaffIds);

                try
                {
                    _patientContext.AddTreatmentSchedule(patient, newTreatment, docSchedule);
                    await _patientContext.SaveChangesAsync();
                }
                catch (ArgumentException arEx)
                {
                    ViewBag.ErrorMessage = arEx.Message;
                    ViewBag.patientId = patientId;
                    return View();
                }
                catch (InvalidDataException invaEx)
                {
                    ViewBag.ErrorMessage = invaEx.Message;
                    ViewBag.patientId = patientId;
                    return View();
                }

                return RedirectToAction(nameof(TreatmentScheduleManager), new {id = patientId});
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult EditSchedule(int? id, int patientId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null || patient.TreatmentSchedule == null || patient.TreatmentSchedule.First(s => s.Id == id) == null)
            {
                return NotFound();
            }
            var treatment = patient.TreatmentSchedule.First(s => s.Id == id);

            ViewBag.patientId = patientId;

            return View(treatment);
        }
        [HttpPost]
        public async Task<IActionResult> EditSchedule(int id, int patientId, TreatmentScheduleEle treatment)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null || id != treatment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Models.Calendar? docSchedule = _staffContext.GetCalendar(patient.StaffIds);

                try
                {
                    _patientContext.UpdateTreatmentSchedule(patient, id, treatment, docSchedule);
                    await _patientContext.SaveChangesAsync();
                }
                catch (InvalidDataException ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    ViewBag.patientId = patientId;
                    return View();
                }

                return RedirectToAction(nameof(TreatmentScheduleManager), new { id = patientId });
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult DeleteSchedule(int? id, int patientId)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null || patient.TreatmentSchedule == null || patient.TreatmentSchedule.First(s => s.Id == id) == null)
            {
                return NotFound();
            }
            var treatment = patient.TreatmentSchedule.First(s => s.Id == id);

            ViewBag.patientId = patientId;

            return View(treatment);
        }
        [HttpPost, ActionName("DeleteSchedule")]
        public async Task<IActionResult> DeleScheduleConfirmed(int id, int patientId)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try {
                    _patientContext.DeleteTreatmentSchedule(patient, id);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return RedirectToAction(nameof(TreatmentScheduleManager), new { id = patientId });
                }

                await _patientContext.SaveChangesAsync();
                return RedirectToAction(nameof(TreatmentScheduleManager), new { id = patientId });
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult SetTestResult(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.patientId = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SetTestResult(int patientId, TestResult result)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _patientContext.SetTestResult(patient, result);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return RedirectToAction(nameof(Index));
                }
                await _patientContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult StaffIdsManager(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);
            if (patient == null || 
                patient.TestResult == null || 
                patient.TestResult.Type == null ||
                patient.StaffIds == null)
            {
                return NotFound();
            }

            StaffIdsModel model = new StaffIdsModel { staffIds = patient.StaffIds };

            ViewBag.patientId = id;
            try
            {
                var suitableStaffs = _staffContext.GetSuitableStaffs((SpecialList)patient.TestResult.Type);
                suitableStaffs.Sort();
                ViewBag.suitableStaffs = suitableStaffs;
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction(nameof(Index));
            } 

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStaffIds(int patientId, StaffIdsModel model)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                _patientContext.SetStaffId(patient, model.staffIds);
                await _patientContext.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult MedicalHistoryManager(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
        public IActionResult AddMedicalHistory (int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            ViewBag.patientId = id;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddMedicalHistory (int patientId, MedicalHistoryEle history)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (history == null)
                {
                    ViewBag.ErrorMessage = "Bad request!";
                    return RedirectToAction(nameof(MedicalHistoryManager), new { id = patientId });
                }

                try
                {
                    _patientContext.AddMedicalHistory(patient, history);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return RedirectToAction(nameof(MedicalHistoryManager), new { id = patientId });
                }

                await _patientContext.SaveChangesAsync();
                return RedirectToAction(nameof(MedicalHistoryManager), new { id = patientId });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMedicalHistory (int patientId, string startDate)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == patientId);
            if (patient == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _patientContext.DeleteMedicalHistory(patient, startDate);
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex.Message;
                    return RedirectToAction(nameof(MedicalHistoryManager), new { id = patientId });
                }

                await _patientContext.SaveChangesAsync();
                return RedirectToAction(nameof(MedicalHistoryManager), new { id = patientId });
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
