using Microsoft.AspNetCore.Mvc;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authorization;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
    public class PatientController : Controller
    {
        public readonly PatientContext _context;
        public PatientController(PatientContext context)
        {
            _context = context;
        }

        // GET: Patient
        public async Task<IActionResult> Index()
        {
            await _context.InitializePatientListFromFirestore();
            return View(PatientContext.PatientList);
        }

        // GET: Patient/Details/3
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
                _context.Add(patient);
                await _context.SaveChangesAsync();

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
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!PatientExists((int)patient.Id)) {
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
            return View(patient);
        }

        //GET: Patient/Remove/3
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

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(int id)
        {
            return PatientContext.PatientList.Any(patient => patient.Id == id);
        }


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
        public IActionResult AddSchedule(int id)
        {
            var patient = PatientContext.PatientList
                .FirstOrDefault(m => m.Id == id);
            if (patient == null)
            {
                return NotFound();
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

                _context.AddTreatmentSchedule(patient, newTreatment);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TreatmentScheduleManager), new {id = patientId});
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
            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditSchedule(int? id, int patientId)
        {
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
                _context.UpdateTreatmentSchedule(patient, id, treatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TreatmentScheduleManager), new { id = patientId });
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
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteSchedule(int? id, int patientId)
        {
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
                _context.DeleteTreatmentSchedule(patient, id);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TreatmentScheduleManager), new { id = patientId });
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
            return RedirectToAction(nameof(Index));
        }

    }
}
