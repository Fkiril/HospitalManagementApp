using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;


namespace HospitalManagementApp.Controllers
{
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
        public async Task<IActionResult> Add(
            [Bind("Id,docId,Name,Gender,DataOfBirth,Address,PhoneNum,MedicalHistory,TestResult,TreatmentSchedule,Status")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,docId,Name,Gender,DataOfBirth,Address,PhoneNum,MedicalHistory,TestResult,TreatmentSchedule,Status")] Patient patient)
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

                return RedirectToAction(nameof(Edit));
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
    }
}
