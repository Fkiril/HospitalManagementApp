using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;


namespace HospitalManagementApp.Controllers
{
    public class PrescriptionController : Controller
    {
        public readonly PrescriptionContext _context;
        public PrescriptionController(PrescriptionContext context)
        {
            _context = context;
        }

        public IActionResult Search(int? searchingId)
        {
            if (!searchingId.HasValue)
            {
                return RedirectToAction(nameof(List));
            }
            else
            {
                var prescriptions = PrescriptionContext.PrescriptionList.Where(p => p.IdOfPatient == searchingId).ToList();

                return View(prescriptions);
            }
        }


        public async Task<IActionResult> List()
        {
            await _context.InitializePrescriptionListFromFirestore();
            return View(PrescriptionContext.PrescriptionList);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = PrescriptionContext.PrescriptionList.FirstOrDefault(prescription => prescription.Id == id);
            if (prescription == null)
            {
                return NotFound();
            }

            return View(prescription);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            [Bind("Id,IdOfPatient,docId,Drug,Description")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(prescription);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(List));
                return View();
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = PrescriptionContext.PrescriptionList
                .FirstOrDefault(prescription => prescription.Id == id);
            if (prescription == null)
            {
                return NotFound();
            }
            return View(prescription);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,IdOfPatient,docId,Drug,Description")] Prescription prescription)
        {
            if (id != prescription.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(prescription);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!PrescriptionExists((int)prescription.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(List));
            }
            return View(prescription);
        }


        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var prescription = PrescriptionContext.PrescriptionList.FirstOrDefault(m => m.Id == id);
            if (prescription == null)
            {
                return NotFound();
            }

            await _context.DeleteAsync(prescription.docId);

            return RedirectToAction(nameof(List));
        }


        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var prescription = PrescriptionContext.PrescriptionList.FirstOrDefault(prescription => prescription.Id == id);
            if (prescription != null)
            {
                PrescriptionContext.PrescriptionList.Remove(prescription);
                await _context.DeleteAsync(prescription.docId);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(List));
            }

            return NotFound();
        }

        [HttpGet, ActionName("GetPrescriptions")]
        public async Task<IActionResult> GetPrescriptions(int Id)
        {
            var prescriptions = await _context.GetPrescriptionsForPatientAsync(Id);
            return View(prescriptions);
        }
        private bool PrescriptionExists(int id)
        {
            return PrescriptionContext.PrescriptionList.Any(prescription => prescription.Id == id);
        }
    }
}