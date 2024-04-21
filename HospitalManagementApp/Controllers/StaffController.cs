using Microsoft.AspNetCore.Mvc;
using Google.Cloud.Firestore;
using HospitalManagementApp.Data;
using HospitalManagementApp.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Google.Api;


namespace HospitalManagementApp.Controllers
{
    [Authorize(Roles = "Admin, Doctor", AuthenticationSchemes = "Cookies")]
    public class StaffController : Controller
    {
        public readonly StaffContext _context;
        public StaffController(StaffContext context)
        {
            _context = context;
        }

        // GET: Staff
        public async Task<IActionResult> Index()
        {
            await _context.InitializeStaffListFromFirestore();
            return View(StaffContext.StaffList);
        }

        // GET: Staff/Details/3
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = StaffContext.StaffList.FirstOrDefault(staff => staff.Id == id);
            if (staff == null)
            {
                return NotFound();
            }

            return View(staff);
        }

        // GET: Staff/Add
        public IActionResult Add()
        {
            return View();
        }
        //POST: Staff/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add( Staff staff)
        {
            if (ModelState.IsValid)
            {
                _context.Add(staff);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        //GET: Staff/Edit/3
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = StaffContext.StaffList
                .FirstOrDefault(staff => staff.Id == id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }
        //POST: Staff/Edit/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( int id, Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!StaffExists((int)staff.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Edit));
            }
            return View(staff);
        }

        //GET: Staff/Remove/3
        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var staff = StaffContext.StaffList
                .FirstOrDefault(s => s.Id == id);
            if (staff == null)
            {
                return NotFound();
            }
            return View(staff);
        }
        //POST: Staff/Remove/3
        [HttpPost, ActionName("Remove")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveConfirmed(int id)
        {
            var staff = StaffContext.StaffList
                .FirstOrDefault(staff => staff.Id == id);
            if (staff != null)
            {
                StaffContext.StaffList.Remove(staff);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ShowPatient(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }
            var staff = StaffContext.StaffList
                  .FirstOrDefault(staff => staff.Id == id);
            if (staff != null)
            {
                var patListNN = PatientContext.PatientList.Where(patient => patient.StaffIds is not null).ToList();
                var patList = patListNN.Where(predicate: patient => patient.StaffIds.Contains((int)id)).ToList();
                if (patListNN == null) return View(staff);
                else return View(patListNN);
            }
            else
            {
                return NotFound();
            }
        }


        private bool StaffExists(int id)
        {
            return StaffContext.StaffList.Any(staff => staff.Id == id);
        }
    
        //GET: Staff/Calendar/3
        public IActionResult Calendar(int id)
        {
            var staff = StaffContext.StaffList
                .FirstOrDefault(staff => staff.Id == id);
            if (staff == null)
            {
                return NotFound();
            }
            else
            {
                if (staff.WorkSchedule == null)
                    throw new Exception("No calendar");
            }
            return View(staff);
        }

        //POST: Staff/Calendar/3
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Calendar (int id,Staff staff)
        {
            if (id != staff.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    if (!StaffExists((int)staff.Id))
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
            return View(staff);
        }
    
        public async Task<IActionResult> CreateCalendar()
        {

            if (ModelState.IsValid)
            {
                _context.CreateCalendar();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
