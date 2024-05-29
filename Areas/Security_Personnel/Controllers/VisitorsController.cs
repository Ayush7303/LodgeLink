using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LodgeLink.Data;
using LodgeLink.Models;

namespace LodgeLink.Areas.Security_Personnel.Controllers
{
    public class VisitorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VisitorsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Security_Personnel/Visitors
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.visitors.Where(v=>v.CheckOutTime==null).Include(v => v.Property).Include(v=>v.Property.Building);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> VisitorLog()
        {
            var applicationDbContext = _context.visitors.Include(v => v.Property).Include(v => v.Property.Building);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Security_Personnel/Visitors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.visitors == null)
            {
                return NotFound();
            }

            var visitor = await _context.visitors
                .Include(v => v.Property)
                .FirstOrDefaultAsync(m => m.VisitorId == id);
            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // GET: Security_Personnel/Visitors/Create
        public IActionResult Create()
        {
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber");
            return View();
        }

        // POST: Security_Personnel/Visitors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visitor visitor,IFormFile? ImageURL)
        {

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (ImageURL != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ImageURL.FileName);
                string visitorPath = Path.Combine(wwwRootPath, @"images\Visitor");
                using (var filestream = new FileStream(Path.Combine(visitorPath, filename), FileMode.Create))
                {
                    ImageURL.CopyTo(filestream);
                }
                visitor.ImageURL = @"\images\Visitor\" + filename;
            }
            visitor.VisitDate = DateTime.Now;
            visitor.CheckInTime = DateTime.Now;

            visitor.ApprovedBy = HttpContext.Session.GetInt32("UserId");
            if (ModelState.IsValid)
            {
                //File Uploads
                

                _context.Add(visitor);
                await _context.SaveChangesAsync();
                TempData["success"] = "Visitor Created Successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", visitor.PropertyId);
            return View(visitor);
        }

        // GET: Security_Personnel/Visitors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.visitors == null)
            {
                return NotFound();
            }

            var visitor = await _context.visitors.FindAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", visitor.PropertyId);
            return View(visitor);
        }

        // POST: Security_Personnel/Visitors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisitorId,PropertyId,VisitorName,ImageURL,VisitDate,Purpose,ContactNumber,VehicleDetails,ApprovedBy,CheckInTime,CheckOutTime")] Visitor visitor)
        {
            if (id != visitor.VisitorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitor);
                    TempData["success"] = "CheckOut Time Added Successfully.";

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorExists(visitor.VisitorId))
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
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", visitor.PropertyId);
            return View(visitor);
        }

        // GET: Security_Personnel/Visitors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.visitors == null)
            {
                return NotFound();
            }

            var visitor = await _context.visitors
                .Include(v => v.Property)
                .FirstOrDefaultAsync(m => m.VisitorId == id);
            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // POST: Security_Personnel/Visitors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.visitors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.visitors'  is null.");
            }
            var visitor = await _context.visitors.FindAsync(id);
            if (visitor != null)
            {
                _context.visitors.Remove(visitor);
            }
            
            await _context.SaveChangesAsync();
            TempData["success"] = "Visitor Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public IActionResult Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                // If the search term is empty, return all data
                var allVisitors = _context.visitors.Include(v => v.Property).Include(v => v.Property.Building).ToList();
                return PartialView("_VisitorListPartial", allVisitors);
            }
            var query = _context.visitors.Include(v => v.Property).Include(v => v.Property.Building)
                             .Where(v => v.VisitorName.Contains(searchTerm) || v.ContactNumber.Contains(searchTerm) || v.Property.PeopertyNumber.Contains(searchTerm) || v.VehicleDetails.Contains(searchTerm) || v.Purpose.Contains(searchTerm))
                             .ToList();

            return PartialView("_VisitorListPartial", query);
        }
        private bool VisitorExists(int id)
        {
          return (_context.visitors?.Any(e => e.VisitorId == id)).GetValueOrDefault();
        }
    }
}
