using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LodgeLink.Data;
using LodgeLink.Models;

namespace LodgeLink.Areas.RequestManager.Controllers
{
    public class MaintenanceManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceManagementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: RequestManager/MaintenanceManagement
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.maintenanceRequests.Where(e=>e.Status.Equals("Pending")).Include(m => m.Property).Include(m => m.Resident).Include(m=>m.Property.Building).Include(m => m.User);
            return View(await applicationDbContext.ToListAsync());
        }
        public async Task<IActionResult> CompletedIndex()
        {
            var applicationDbContext = _context.maintenanceRequests.Where(e => e.Status.Equals("Completed")).Include(m => m.Property).Include(m => m.Resident).Include(m=>m.Property.Building).Include(m => m.User);
            return View(await applicationDbContext.ToListAsync());
        }
        // GET: RequestManager/MaintenanceManagement/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.maintenanceRequests == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.maintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Resident)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // GET: RequestManager/MaintenanceManagement/Create
        public IActionResult Create()
        {
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber");
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email");
            ViewData["UserId"] = new SelectList(_context.users, "UserId", "Password");
            return View();
        }

        // POST: RequestManager/MaintenanceManagement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,ResidentId,PropertyId,RequestDate,Description,Status,UserId,CompletionDate,Priority,Category,Notes,Attachments")] MaintenanceRequest maintenanceRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenanceRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", maintenanceRequest.PropertyId);
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email", maintenanceRequest.ResidentId);
            ViewData["UserId"] = new SelectList(_context.users, "UserId", "Password", maintenanceRequest.UserId);
            return View(maintenanceRequest);
        }

        // GET: RequestManager/MaintenanceManagement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.maintenanceRequests == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.maintenanceRequests.FindAsync(id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }
            var data = from s in _context.users
                       where s.RoleId == 8
                       select s;
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", maintenanceRequest.PropertyId);
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email", maintenanceRequest.ResidentId);
            ViewData["UserId"] = new SelectList(data, "UserId", "UserName", maintenanceRequest.UserId);
            return View(maintenanceRequest);
        }

        // POST: RequestManager/MaintenanceManagement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,ResidentId,PropertyId,RequestDate,Description,Status,UserId,CompletionDate,Priority,Category,Notes,Attachments")] MaintenanceRequest maintenanceRequest)
        {
            if (id != maintenanceRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(maintenanceRequest);
                    TempData["success"] = "Staff Assigned Successfully.";

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceRequestExists(maintenanceRequest.RequestId))
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
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", maintenanceRequest.PropertyId);
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email", maintenanceRequest.ResidentId);
            ViewData["UserId"] = new SelectList(_context.users, "UserId", "UserName", maintenanceRequest.UserId);
            return View(maintenanceRequest);
        }

        // GET: RequestManager/MaintenanceManagement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.maintenanceRequests == null)
            {
                return NotFound();
            }

            var maintenanceRequest = await _context.maintenanceRequests
                .Include(m => m.Property)
                .Include(m => m.Resident)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (maintenanceRequest == null)
            {
                return NotFound();
            }

            return View(maintenanceRequest);
        }

        // POST: RequestManager/MaintenanceManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.maintenanceRequests == null)
            {
                return Problem("Entity set 'ApplicationDbContext.maintenanceRequests'  is null.");
            }
            var maintenanceRequest = await _context.maintenanceRequests.FindAsync(id);
            if (maintenanceRequest != null)
            {
                _context.maintenanceRequests.Remove(maintenanceRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MaintenanceRequestExists(int id)
        {
          return (_context.maintenanceRequests?.Any(e => e.RequestId == id)).GetValueOrDefault();
        }
    }
}
