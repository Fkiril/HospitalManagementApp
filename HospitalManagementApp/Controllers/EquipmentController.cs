using Microsoft.AspNetCore.Mvc;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin, SupportStaff", AuthenticationSchemes = "Cookies")]
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

            var equipment = EquipmentContext.EquipmentList.FirstOrDefault(equipment => equipment.Id == id);
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
                .FirstOrDefault(equipment => equipment.Id == id);
            if (equipment != null)
            {
                _context.Remove(equipment);
                EquipmentContext.EquipmentList.Remove(equipment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult AddSchedule(int? id)
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

            ViewBag.EquipmentId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSchedule(int id, DateTime schedule)
        {
            if (ModelState.IsValid)
            {
                if (schedule < DateTime.Now)
                {
                    ViewData["AlertMessage"] = "You must choose a time in the future";
                    return View();
                }

                var equipment = EquipmentContext.EquipmentList.FirstOrDefault(e => e.Id == id);
                if (equipment == null)
                {
                    throw new Exception("Add Schedule bug, can't find id " + id);
                }

                equipment.History ??= new List<string>();
                Console.WriteLine("I'm gonna add " + schedule);
                foreach (var time in equipment.History)
                {
                    DateTime startTime = DateTime.Parse(time);
                    if (schedule >= startTime && schedule <= startTime.AddDays(7))
                    {
                        ViewData["AlertMessage"] = "Can't add this date because there will be another patient using it";
                        return View();
                    }
                }

                var utcSchedule = schedule.ToUniversalTime();
                _context.AddSchedule(id, utcSchedule);
                return RedirectToAction(nameof(Index));
            }

            return View();
        }

        private bool EquipmentExists(int id)
        {
            return EquipmentContext.EquipmentList.Any(equipment => equipment.Id == id);
        }
    }
}
