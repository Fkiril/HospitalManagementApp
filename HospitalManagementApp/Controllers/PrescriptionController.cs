using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
    public class PrescriptionController : Controller
    {
        public readonly PrescriptionContext _presciptionContext;
        public readonly DrugsContext _drugContext;
        private PatientContext _patientContext;
        public PrescriptionController(PrescriptionContext context, DrugsContext drugContext, PatientContext patientContext)
        {
            _presciptionContext = context;
            _drugContext = drugContext;
            _patientContext = patientContext;
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
            try
            {
                await _context.InitializePrescriptionListFromFirestore();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            
            return View(PrescriptionContext.PrescriptionList);
        }

        public IActionResult Details(int? id)
        {
            Prescription? prescription = new();
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                prescription = PrescriptionContext.PrescriptionList.FirstOrDefault(prescription => prescription.Id == id);
                if (prescription == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return View(prescription);
        }

        public async Task<IActionResult> Add()
        {
            Prescription pers = new();
            await _patientContext.InitializePatientListFromFirestore();
            ViewData["PatientList"] = PatientContext.PatientList;
            return View(pers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            [Bind("Id,IdOfPatient,docId,Drug,Description")] Prescription prescription)
        {
            if (ModelState.IsValid)
            {
                try
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
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                
            }
            return RedirectToAction("Add");
        }

        [HttpGet]
        public async Task<IActionResult> AddDetail(int presId)
        {
            try
            {
                await _drugContext.InitializeDrugsListFromFirestore();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            ViewData["PrescriptionId"] = presId;
            ViewData["ListDrug"] = DrugsContext.DrugsList;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDetail(DrugInfo model)
        {
            Drugs drug = await _drugContext.FindById(model.IdOfDrug ?? -1);
            model.NameOfDrug = drug.Name;
            if(drug.Quantity < model.Quantity)
                TempData["error"] = "The quantity of medicine is not enough";
            else
            {
                try
                {
                    drug.Quantity -= model.Quantity;
                    _drugContext.Update(drug);
                    await _drugContext.SaveChangesAsync();
                    await _context.AddDetail(model);
                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                
            }    
           
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
                    _presciptionContext.Update(prescription);
                    await _presciptionContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (!PrescriptionExists((int)prescription.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw new Exception(ex.Message, ex);
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

            try
            {
                await _context.DeleteAsync(prescription.docId ?? "");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            return RedirectToAction(nameof(List));
        }

        public async Task<IActionResult> Remove_v2(int? id)
        {
            try
            {
                var prescription = PrescriptionContext.PrescriptionList.FirstOrDefault(m => m.Id == id);
                if (prescription != null)
                {
                    await _context.DeleteAsync(prescription.docId ?? "");
                }
            }
            catch (Exception ex)
            {
                return Json("fail");
            }

            return Json("ok");
        }


        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var prescription = PrescriptionContext.PrescriptionList.FirstOrDefault(prescription => prescription.Id == id);
            if (prescription != null)
            {
                try
                {
                    PrescriptionContext.PrescriptionList.Remove(prescription);
                    await _context.DeleteAsync(prescription.docId ?? "");

                    await _context.SaveChangesAsync();
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                

                return RedirectToAction(nameof(List));
            }

            return NotFound();
        }

        [HttpGet, ActionName("GetPrescriptions")]
        public async Task<IActionResult> GetPrescriptions(int Id)
        {
            try
            {
                IEnumerable<Prescription> prescriptions = await _context.GetPrescriptionsForPatientAsync(Id);
                return View(prescriptions);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
        private bool PrescriptionExists(int id)
        {
            return PrescriptionContext.PrescriptionList.Any(prescription => prescription.Id == id);
        }
    }
}