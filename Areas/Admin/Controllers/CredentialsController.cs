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
    public class CredentialsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CredentialsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Credentials
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.credentials.Include(c => c.Building).Include(c => c.Property);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Credentials/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.credentials == null)
            {
                return NotFound();
            }

            var credentials = await _context.credentials
                .Include(c => c.Building)
                .Include(c => c.Property)
                .FirstOrDefaultAsync(m => m.CredentialId == id);
            if (credentials == null)
            {
                return NotFound();
            }

            return View(credentials);
        }

        // GET: Credentials/Create
        public IActionResult Create()
        {
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName");
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber");
            return View();
        }

        // POST: Credentials/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CredentialId,BuildingId,PropertyId,Username,Password")] Credentials credentials)
        {
            if (ModelState.IsValid)
            {
                _context.Add(credentials);
                await _context.SaveChangesAsync();
                TempData["success"] = "Credentials Created Successfully.";

                return RedirectToAction(nameof(Index));
            }
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName", credentials.BuildingId);
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", credentials.PropertyId);
            return View(credentials);
        }

        // GET: Credentials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.credentials == null)
            {
                return NotFound();
            }

            var credentials = await _context.credentials.FindAsync(id);
            if (credentials == null)
            {
                return NotFound();
            }
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName", credentials.BuildingId);
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", credentials.PropertyId);
            return View(credentials);
        }

        // POST: Credentials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CredentialId,BuildingId,PropertyId,Username,Password")] Credentials credentials)
        {
            if (id != credentials.CredentialId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credentials);
                    TempData["success"] = "Credential Updated Successfully.";

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialsExists(credentials.CredentialId))
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
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName", credentials.BuildingId);
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", credentials.PropertyId);
            return View(credentials);
        }

        // GET: Credentials/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.credentials == null)
            {
                return NotFound();
            }

            var credentials = await _context.credentials
                .Include(c => c.Building)
                .Include(c => c.Property)
                .FirstOrDefaultAsync(m => m.CredentialId == id);
            if (credentials == null)
            {
                return NotFound();
            }

            return View(credentials);
        }

        // POST: Credentials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.credentials == null)
            {
                return Problem("Entity set 'ApplicationDbContext.credentials'  is null.");
            }
            var credentials = await _context.credentials.FindAsync(id);
            if (credentials != null)
            {
                _context.credentials.Remove(credentials);
            }
            TempData["success"] = "Credential Deleted Successfully.";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CredentialsExists(int id)
        {
            return (_context.credentials?.Any(e => e.CredentialId == id)).GetValueOrDefault();
        }
    }
}
