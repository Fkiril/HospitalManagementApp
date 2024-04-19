using Microsoft.AspNetCore.Mvc;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin", AuthenticationSchemes = "Cookies")]
    public class EquipmentController : Controller
    {
        public readonly EquipmentContext _context;
        public EquipmentController(EquipmentContext context)
        {
            _context = context;
        }

        // GET: Equipment
        public async Task<IActionResult> Index()
        {
            await _context.InitializeEquipmentListFromFirestore();
            return View(EquipmentContext.EquipmentList);
        }

        // GET: Equipment/Details/3
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = EquipmentContext.EquipmentList.FirstOrDefault(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }

        // GET: Equipment/Add
        public IActionResult Add()
        {
            return View();
        }
        //POST: Equipment/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(equipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        //GET: Equipment/Edit/3
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = EquipmentContext.EquipmentList
                .FirstOrDefault(equipment => equipment.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            return View(equipment);
        }
        //POST: Equipment/Edit/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Equipment equipment)
        {
            if (id != equipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(equipment);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!EquipmentExists((int)equipment.Id))
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
            return View(equipment);
        }

        //GET: Equipment/Remove/3
        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var equipment = EquipmentContext.EquipmentList
                .FirstOrDefault(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }
            return View(equipment);
        }
        //POST: Equipment/Remove/3
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var equipment = EquipmentContext.EquipmentList
                .FirstOrDefault(m => m.Id == id);
            if (equipment != null)
            {
                EquipmentContext.EquipmentList.Remove(equipment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EquipmentExists(int id)
        {
            return EquipmentContext.EquipmentList.Any(m => m.Id == id);
        }

        public IActionResult AddSchedule(int? id)
        {
            var equipment = EquipmentContext.EquipmentList
                .FirstOrDefault(m => m.Id == id);
            if (equipment == null)
            {
                return NotFound();
            }

            ViewBag.EquipmentId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSchedule(int equipmentId, DateTime newSchedule)
        {
            var equipment = EquipmentContext.EquipmentList
                .FirstOrDefault(m => m.Id == equipmentId);
            if (equipment == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (newSchedule == null)
                {
                    return BadRequest();
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
