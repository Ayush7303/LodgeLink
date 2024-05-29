using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LodgeLink.Data;
using LodgeLink.Models;

namespace LodgeLink.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.properties.Include(x => x.Building);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.properties == null)
            {
                return NotFound();
            }

            var @property = await _context.properties
                .Include(x => x.Building)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName");
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyId,PeopertyNumber,PropertyType,PropertySize,PropertyStatus,PurchaseDate,PropertyValue,PropertyFeatures,BuildingId,Description")] Property @property)
        {
            if (ModelState.IsValid)
            {
                
                
                var bame = (from b in _context.buildings
                           where b.BuildingId == property.BuildingId
                           select b.BuildingName).FirstOrDefault();
                @property.PeopertyNumber = bame + @property.PeopertyNumber;

                if (_context.properties.Any(p => p.PeopertyNumber == property.PeopertyNumber) && _context.buildings.Any(p => p.BuildingId == property.BuildingId))
                {
                    TempData["error"] = "Property Already Exists.";
                    ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName", @property.BuildingId);

                    return View(@property);
                }
                property.PurchaseDate=DateTime.Now;
                _context.Add(@property);
                //await _context.Database.ExecuteSqlRawAsync("DISABLE TRIGGER insertincred ON properties");
                await _context.SaveChangesAsync();
                TempData["success"] = "Property Created Successfully.";
                TempData["success"] = "Credential Created Successfully.";
                //await _context.Database.ExecuteSqlRawAsync("ENABLE TRIGGER insertincred ON properties");
                return RedirectToAction(nameof(Index));
            }
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName", @property.BuildingId);
            return View(@property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.properties == null)
            {
                return NotFound();
            }

            var @property = await _context.properties.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName", @property.BuildingId);
            return View(@property);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyId,PeopertyNumber,PropertyType,PropertySize,PropertyStatus,PurchaseDate,PropertyValue,PropertyFeatures,BuildingId,Description")] Property @property)
        {
            if (id != @property.PropertyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@property);
                    TempData["success"] = "Property Updated Successfully.";

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.PropertyId))
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
            ViewData["BuildingId"] = new SelectList(_context.buildings, "BuildingId", "BuildingName", @property.BuildingId);
            return View(@property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.properties == null)
            {
                return NotFound();
            }

            var @property = await _context.properties
                .Include(x => x.Building)
                .FirstOrDefaultAsync(m => m.PropertyId == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.properties == null)
            {
                return Problem("Entity set 'ApplicationDbContext.properties'  is null.");
            }
            var @property = await _context.properties.FindAsync(id);
            if (@property != null)
            {
                _context.properties.Remove(@property);
            }
            
            await _context.SaveChangesAsync();
            TempData["success"] = "Property Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
          return (_context.properties?.Any(e => e.PropertyId == id)).GetValueOrDefault();
        }
    }
}
