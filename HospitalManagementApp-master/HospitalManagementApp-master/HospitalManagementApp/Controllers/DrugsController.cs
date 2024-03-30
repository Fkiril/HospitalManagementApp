using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;


namespace HospitalManagementApp.Controllers
{
    public class DrugsController : Controller
    {
        public readonly DrugsContext _context;
        public DrugsController(DrugsContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            await _context.InitializeDrugsListFromFirestore();
            return View(DrugsContext.DrugsList);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugs = DrugsContext.DrugsList.FirstOrDefault(drugs => drugs.Id == id);
            if (drugs == null)
            {
                return NotFound();
            }

            return View(drugs);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(
            [Bind("Id,docId,Name,HisUse,Expiry,Status")] Drugs drugs)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drugs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugs = DrugsContext.DrugsList
                .FirstOrDefault(drugs => drugs.Id == id);
            if (drugs == null)
            {
                return NotFound();
            }
            return View(drugs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("Id,docId,Name,HisUse,Expiry,Status")] Drugs drugs)
        {
            if (id != drugs.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drugs);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!DrugsExists((int)drugs.Id))
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
            return View(drugs);
        }

        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = DrugsContext.DrugsList
                .FirstOrDefault(m => m.Id == id);
            if (drug == null)
            {
                return NotFound();
            }
            return View(drug);
        }

        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var drug = DrugsContext.DrugsList
                .FirstOrDefault(drug => drug.Id == id);
            if (drug != null)
            {
                DrugsContext.DrugsList.Remove(drug);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool DrugsExists(int id)
        {
            return DrugsContext.DrugsList.Any(drugs => drugs.Id == id);
        }
    }
}