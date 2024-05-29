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
    public class BillsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Bills
        public async Task<IActionResult> Index()
        {
              return _context.bills != null ? 
                          View(await _context.bills.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.bills'  is null.");
        }

        // GET: Admin/Bills/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.bills == null)
            {
                return NotFound();
            }

            var bill = await _context.bills
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // GET: Admin/Bills/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Bills/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillId,Title,DateRange,DueDate,Paid,Unpaid,CreatedAt,UpdatedAt")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                bill.CreatedAt = DateTime.Now;
                bill.UpdatedAt = DateTime.Now;
                bill.Unpaid = _context.residents.Count();
                bill.Title = "Invoice (" + bill.DateRange + ")";
                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }

        // GET: Admin/Bills/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.bills == null)
            {
                return NotFound();
            }

            var bill = await _context.bills.FindAsync(id);
            if (bill == null)
            {
                return NotFound();
            }
            return View(bill);
        }

        // POST: Admin/Bills/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillId,Title,DateRange,DueDate,Paid,Unpaid,CreatedAt,UpdatedAt")] Bill bill)
        {
            if (id != bill.BillId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    bill.UpdatedAt=DateTime.Now;
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillId))
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
            return View(bill);
        }

        // GET: Admin/Bills/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.bills == null)
            {
                return NotFound();
            }

            var bill = await _context.bills
                .FirstOrDefaultAsync(m => m.BillId == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Admin/Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.bills == null)
            {
                return Problem("Entity set 'ApplicationDbContext.bills'  is null.");
            }
            var bill = await _context.bills.FindAsync(id);
            if (bill != null)
            {
                _context.bills.Remove(bill);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
          return (_context.bills?.Any(e => e.BillId == id)).GetValueOrDefault();
        }
    }
}
