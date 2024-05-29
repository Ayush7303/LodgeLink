using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Hosting;

namespace LodgeLink.Areas.Admin.Controllers
{
    public class ResidentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ResidentsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        // GET: Residents
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.residents.Include(r => r.Credentials).Include(r => r.Property);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Residents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.residents == null)
            {
                return NotFound();
            }

            var resident = await _context.residents
                .Include(r => r.Credentials)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(m => m.ResidentId == id);
            if (resident == null)
            {
                return NotFound();
            }

            return View(resident);
        }

        // GET: Residents/Create
        public IActionResult Create()
        {
            ViewData["CredentialId"] = new SelectList(_context.credentials, "CredentialId", "CredentialId");
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber");
            return View();
        }

        // POST: Residents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResidentId,FirstName,LastName,ContactNumber,Email,PropertyId,CredentialId,DateOfBirth,DateOfMoveIn,EmergencyContactNumber,ProfileImage,IsActive,CreatedAt,UpdatedAt,Status")] Resident resident)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resident);
                await _context.SaveChangesAsync();
                TempData["success"] = "Registered Successfully.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["CredentialId"] = new SelectList(_context.credentials, "CredentialId", "CredentialId", resident.CredentialId);
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", resident.PropertyId);
            return View(resident);
        }

        // GET: Residents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.residents == null)
            {
                return NotFound();
            }

            var resident = await _context.residents.FindAsync(id);
            if (resident == null)
            {
                return NotFound();
            }
            ViewData["CredentialId"] = new SelectList(_context.credentials, "CredentialId", "CredentialId", resident.CredentialId);
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", resident.PropertyId);
            return View(resident);
        }

        // POST: Residents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResidentId,FirstName,LastName,ContactNumber,Email,PropertyId,CredentialId,DateOfBirth,DateOfMoveIn,EmergencyContactNumber,ProfileImage,IsActive,CreatedAt,UpdatedAt,Status")] Resident resident,IFormFile? ImageURL)
        {
            if (id != resident.ResidentId)
            {
                return NotFound();
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (ImageURL != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ImageURL.FileName);
                string productpath = Path.Combine(wwwRootPath, @"images\Resident");
                if (!string.IsNullOrEmpty(resident.ProfileImage))
                {
                    //old image delete
                    var oldpath = Path.Combine(wwwRootPath, resident.ProfileImage.TrimStart('\\'));
                    if (System.IO.File.Exists(oldpath))
                    {
                        System.IO.File.Delete(oldpath);
                    }
                }
                using (var filestream = new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                {
                    ImageURL.CopyTo(filestream);
                }
                resident.ProfileImage = @"\images\Resident\" + filename;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    resident.UpdatedAt = DateTime.Now;
                    _context.Update(resident);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResidentExists(resident.ResidentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "Resident Updated Successfully.";

                return RedirectToAction(nameof(Index));
            }
            ViewData["CredentialId"] = new SelectList(_context.credentials, "CredentialId", "CredentialId", resident.CredentialId);
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", resident.PropertyId);
            return View(resident);
        }

        // GET: Residents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.residents == null)
            {
                return NotFound();
            }

            var resident = await _context.residents
                .Include(r => r.Credentials)
                .Include(r => r.Property)
                .FirstOrDefaultAsync(m => m.ResidentId == id);
            if (resident == null)
            {
                return NotFound();
            }

            return View(resident);
        }

        // POST: Residents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.residents == null)
            {
                return Problem("Entity set 'ApplicationDbContext.residents'  is null.");
            }
            var resident = await _context.residents.FindAsync(id);
            if (resident != null)
            {
                _context.residents.Remove(resident);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Resident Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }

        private bool ResidentExists(int id)
        {
            return (_context.residents?.Any(e => e.ResidentId == id)).GetValueOrDefault();
        }
    }
}
