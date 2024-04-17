using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Mvc;


namespace HospitalManagementApp.Controllers
{
    public class DrugsController : Controller
    {
        public readonly DrugsContext _context;
        public DrugsController(DrugsContext context)
        {
            _context = context;
        }

        public IActionResult Search(string SearchingString)
        {
            if (SearchingString == null)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var alldrug = DrugsContext.DrugsList.Where(n => n.Name.Contains(SearchingString)).ToList();
                return View(alldrug);
            }
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

            var drugs = DrugsContext.DrugsList.FirstOrDefault(drugs => drugs.IdOfDrug == id);
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
            [Bind("IdOfDrug,docId,Name,HisUse,Expiry,Status,Quantity,ReceiptDay")] Drugs drugs)
        {
            if (ModelState.IsValid)
            {
                if (IsIdOfDrugExists(drugs.IdOfDrug))
                {
                    // Nếu IdOfDrug đã tồn tại, hiển thị thông báo lỗi
                    ModelState.AddModelError(string.Empty, "IdOfDrug already exists.");
                    return View(drugs);
                }
                _context.Add(drugs);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private bool IsIdOfDrugExists(int? idOfDrug)
        {
            if (idOfDrug == null)
            {
                return false;
            }

            var existingIds = DrugsContext.DrugsList.Select(d => d.IdOfDrug);
            return existingIds.Contains(idOfDrug);
        }


        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drugs = DrugsContext.DrugsList
                .FirstOrDefault(drugs => drugs.IdOfDrug == id);
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
            [Bind("IdOfDrug,docId,Name,HisUse,Expiry,Status,Quantity,ReceiptDay")] Drugs drugs)
        {
            if (id != drugs.IdOfDrug)
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
                    if (!DrugsExists((int)drugs.IdOfDrug))
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


        public async Task<IActionResult> Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drug = DrugsContext.DrugsList.FirstOrDefault(m => m.IdOfDrug == id);
            if (drug == null)
            {
                return NotFound();
            }

            await _context.DeleteAsync(drug.docId);

            return RedirectToAction(nameof(Index));
        }


        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var drug = DrugsContext.DrugsList.FirstOrDefault(drug => drug.IdOfDrug == id);
            if (drug != null)
            {
                DrugsContext.DrugsList.Remove(drug);
                await _context.DeleteAsync(drug.docId);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        private bool DrugsExists(int id)
        {
            return DrugsContext.DrugsList.Any(drugs => drugs.IdOfDrug == id);
        }
    }
}


