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
    public class BuildingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BuildingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Buildings
        public async Task<IActionResult> Index()
        {
            return _context.buildings != null ?
                        View(await _context.buildings.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.buildings'  is null.");
        }

        // GET: Buildings/Details/5
       

        // GET: Buildings/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Buildings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BuildingId,BuildingName,Address,TotalFloors,TotalUnits,BuildingType,ConstructionDate,CreatedAt,UpdatedAt,Description")] Building building)
        {
            if (ModelState.IsValid)
            {
                if (_context.buildings.Any(p => p.BuildingName == building.BuildingName))
                {
                    TempData["error"] = "Building Name Already Exists.";
                

                    return View(building);
                }
                building.CreatedAt = DateTime.Now;
                building.UpdatedAt = DateTime.Now;
                _context.Add(building);
                await _context.SaveChangesAsync();
                TempData["success"] = "Building Created Successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(building);
        }

        // GET: Buildings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.buildings == null)
            {
                return NotFound();
            }

            var building = await _context.buildings.FindAsync(id);
            if (building == null)
            {
                return NotFound();
            }
            return View(building);
        }

        // POST: Buildings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BuildingId,BuildingName,Address,TotalFloors,TotalUnits,BuildingType,ConstructionDate,CreatedAt,UpdatedAt,Description")] Building building)
        {
            if (id != building.BuildingId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    building.UpdatedAt=DateTime.Now;
                    _context.Update(building);
                    TempData["success"] = "Building Updated Successfully.";
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BuildingExists(building.BuildingId))
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
            return View(building);
        }

        // GET: Buildings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.buildings == null)
            {
                return NotFound();
            }

            var building = await _context.buildings
                .FirstOrDefaultAsync(m => m.BuildingId == id);
            if (building == null)
            {
                return NotFound();
            }

            return View(building);
        }

        // POST: Buildings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.buildings == null)
            {
                return Problem("Entity set 'ApplicationDbContext.buildings'  is null.");
            }
            var building = await _context.buildings.FindAsync(id);
            if (building != null)
            {
                _context.buildings.Remove(building);
            }
            TempData["success"] = "Building Deleted Successfully.";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BuildingExists(int id)
        {
            return (_context.buildings?.Any(e => e.BuildingId == id)).GetValueOrDefault();
        }
    }
}
