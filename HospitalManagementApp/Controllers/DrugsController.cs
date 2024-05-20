using Google.Protobuf.WellKnownTypes;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin, Doctor, SupportStaff", AuthenticationSchemes = "Cookies")]
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
                try
                {
                    var alldrug = DrugsContext.DrugsList.Where(n => !string.IsNullOrEmpty(n.Name) ? n.Name.Contains(SearchingString) : true).ToList();
                    return View(alldrug);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                await _context.InitializeDrugsListFromFirestore();
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return View(DrugsContext.DrugsList);
        }

        public IActionResult Details(int? id)
        {
            Drugs? drugs = new();
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                drugs = DrugsContext.DrugsList.FirstOrDefault(drugs => drugs.IdOfDrug == id);
                if (drugs == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
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
            [Bind("IdOfDrug,docId,Name,Status,Quantity,ManufactureDate,ExpirationDate")] Drugs drugs)
        {
            if (ModelState.IsValid)
            {
                if (IsIdOfDrugExists(drugs.IdOfDrug))
                {
                    // Nếu IdOfDrug đã tồn tại, hiển thị thông báo lỗi
                    ModelState.AddModelError(string.Empty, "IdOfDrug already exists.");
                    return View(drugs);
                }
                try
                {
                    drugs.ReceiptDay = DateTime.Now;
                    drugs.Status = Status.Available;
                    _context.Add(drugs);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", ex.Message.ToString());
                    Console.WriteLine("Add_drugcontroler", ex.Message);
                    return View(drugs);
                }
                
            }
            return View();
        }

        private bool IsIdOfDrugExists(int? idOfDrug)
        {
            if (idOfDrug == null)
            {
                return false;
            }

            try
            {
                var existingIds = DrugsContext.DrugsList.Select(d => d.IdOfDrug);
                return existingIds.Contains(idOfDrug);
            }
            catch(Exception ex)
            {
                Console.WriteLine("IsIdOfDrugExists_drugcontroler", ex.Message);
            }
            return false;
        }


        public IActionResult Edit(int? id)
        {
            Drugs? drugs = null;
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                drugs = DrugsContext.DrugsList
                    .FirstOrDefault(drugs => drugs.IdOfDrug == id);
                if (drugs == null)
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("IsIdOfDrugExists_drugcontroler", ex.Message);
            }
            return View(drugs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdOfDrug,docId,Name,Status,Quantity,ManufactureDate,ExpirationDate")] Drugs drugs)
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
                catch (Exception ex)
                {
                    if (!DrugsExists((int)drugs.IdOfDrug))
                    {
                        return NotFound();
                    }
                    else
                    {
                        ModelState.AddModelError("", ex.Message.ToString());
                        return View(drugs);
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
            try
            {
                var drug = DrugsContext.DrugsList.FirstOrDefault(m => m.IdOfDrug == id);
                if (drug == null)
                {
                    return NotFound();
                }

                await _context.DeleteAsync(drug.docId ?? "");
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Remove_v2(int? id)
        {
            try
            {
                var drug = DrugsContext.DrugsList.FirstOrDefault(m => m.IdOfDrug == id);
                if(drug != null)
                    await _context.DeleteAsync(drug.docId ?? "");
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
            var drug = DrugsContext.DrugsList.FirstOrDefault(drug => drug.IdOfDrug == id);
            if (drug != null)
            {
                try
                {
                    DrugsContext.DrugsList.Remove(drug);
                    await _context.DeleteAsync(drug.docId ?? "");

                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message, ex);
                }
                
            }

            return NotFound();
        }

        private bool DrugsExists(int id)
        {
            return DrugsContext.DrugsList.Any(drugs => drugs.IdOfDrug == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetDrugById(int id)
        {
            await _context.InitializeDrugsListFromFirestore();
            Drugs? drug = null;
            try
            {
                drug = DrugsContext.DrugsList.FirstOrDefault(d => d.IdOfDrug.Equals(id));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            return Json(drug);
        }
    }
}