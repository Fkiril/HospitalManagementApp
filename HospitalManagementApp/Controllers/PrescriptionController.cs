using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Mvc;


namespace HospitalManagementApp.Controllers
{
    public class PrescriptionController : Controller
    {
        public readonly PrescriptionContext _context;
        public readonly DrugsContext _drugContext;
        public PrescriptionController(PrescriptionContext context, DrugsContext drugContext)
        {
            _context = context;
            _drugContext = drugContext;
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

        public async Task<IActionResult> Add()
        {
            Prescription pers = new Prescription();
            return View(pers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            [Bind("Id,IdOfPatient,docId,Drug,Description")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                await _context.InitializePrescriptionListFromFirestore();
                if (_context.FindById(prescription.Id ?? -1) != null)
                {
                    TempData["error"] = "This Prescription Id is existed";
                }
                else
                {
                    _context.Add(prescription);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AddDetail", new { presId = prescription.Id });
                }
            }
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AddDetail(int presId)
        {
            ViewData["PrescriptionId"] = presId;
            ViewData["ListDrug"] = await _drugContext.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetail(DrugInfo model)
        {
            Drugs drug = await _drugContext.FindById(model.IdOfDrug ?? -1);
            model.NameOfDrug = drug.Name;
            await _context.AddDetail(model);
            await _context.SaveChangesAsync();
            return RedirectToAction("AddDetail", new { presId = model.PresId });
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