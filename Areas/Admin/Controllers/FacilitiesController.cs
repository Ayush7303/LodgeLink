using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LodgeLink.Data;
using LodgeLink.Models;

namespace LodgeLink.Areas.Admin.Controllers
{
   
    public class FacilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FacilitiesController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        // GET: Admin/Facilities
        public async Task<IActionResult> Index()
        {
              return _context.facilities != null ? 
                          View(await _context.facilities.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.facilities'  is null.");
        }

        // GET: Admin/Facilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.facilities == null)
            {
                return NotFound();
            }

            var facility = await _context.facilities
                .FirstOrDefaultAsync(m => m.FacilityId == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // GET: Admin/Facilities/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Facilities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Facility facility, IFormFile? ImageURL)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (ImageURL != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ImageURL.FileName);
                string visitorPath = Path.Combine(wwwRootPath, @"images\Facility");
                using (var filestream = new FileStream(Path.Combine(visitorPath, filename), FileMode.Create))
                {
                    ImageURL.CopyTo(filestream);
                }
                facility.ImageURL = @"\images\Facility\" + filename;
            }
            if (ModelState.IsValid)
            {
                facility.CreatedDate = DateTime.Now;
                facility.UpdatedDate = DateTime.Now;
                _context.Add(facility);
                await _context.SaveChangesAsync();
                TempData["success"] = "Facility Created Successfully.";

                return RedirectToAction(nameof(Index));
            }
            return View(facility);
        }

        // GET: Admin/Facilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.facilities == null)
            {
                return NotFound();
            }

            var facility = await _context.facilities.FindAsync(id);
            if (facility == null)
            {
                return NotFound();
            }
            return View(facility);
        }

        // POST: Admin/Facilities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Facility facility,IFormFile? ImageURL)
        {
            if (id != facility.FacilityId)
            {
                return NotFound();
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (ImageURL != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ImageURL.FileName);
                string productpath = Path.Combine(wwwRootPath, @"images\Facility");
                if (!string.IsNullOrEmpty(facility.ImageURL))
                {
                    //old image delete
                    var oldpath = Path.Combine(wwwRootPath, facility.ImageURL.TrimStart('\\'));
                    if (System.IO.File.Exists(oldpath))
                    {
                        System.IO.File.Delete(oldpath);
                    }
                }
                using (var filestream = new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                {
                    ImageURL.CopyTo(filestream);
                }
                facility.ImageURL = @"\images\Facility\" + filename;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    facility.UpdatedDate = DateTime.Now;
                    _context.Update(facility);
                    TempData["success"] = "Facility Updated Successfully.";

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacilityExists(facility.FacilityId))
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
            return View(facility);
        }

        // GET: Admin/Facilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.facilities == null)
            {
                return NotFound();
            }

            var facility = await _context.facilities
                .FirstOrDefaultAsync(m => m.FacilityId == id);
            if (facility == null)
            {
                return NotFound();
            }

            return View(facility);
        }

        // POST: Admin/Facilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.facilities == null)
            {
                return Problem("Entity set 'ApplicationDbContext.facilities'  is null.");
            }
            var facility = await _context.facilities.FindAsync(id);
            if (facility != null)
            {
                _context.facilities.Remove(facility);
            }
            
            await _context.SaveChangesAsync();
            TempData["success"] = "Facility Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }

        private bool FacilityExists(int id)
        {
          return (_context.facilities?.Any(e => e.FacilityId == id)).GetValueOrDefault();
        }
    }
}
