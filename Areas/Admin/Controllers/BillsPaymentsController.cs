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
    public class BillsPaymentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BillsPaymentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/BillsPayments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.bills_payment.Include(b => b.Bill).Include(b => b.Resident).Include(x=>x.Resident.Property);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/BillsPayments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.bills_payment == null)
            {
                return NotFound();
            }

            var billsPayment = await _context.bills_payment
                .Include(b => b.Bill)
                .Include(b => b.Resident)
                .FirstOrDefaultAsync(m => m.BillPaymentId == id);
            if (billsPayment == null)
            {
                return NotFound();
            }

            return View(billsPayment);
        }

        // GET: Admin/BillsPayments/Create
        public IActionResult Create()
        {
            ViewData["BillId"] = new SelectList(_context.bills, "BillId", "DateRange");
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email");
            return View();
        }

        // POST: Admin/BillsPayments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillPaymentId,BillId,ResidentId,Amount,PaymentId,PaidAmount,Status,PaidOn")] BillsPayment billsPayment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(billsPayment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BillId"] = new SelectList(_context.bills, "BillId", "DateRange", billsPayment.BillId);
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email", billsPayment.ResidentId);
            return View(billsPayment);
        }

        // GET: Admin/BillsPayments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.bills_payment == null)
            {
                return NotFound();
            }

            var billsPayment = await _context.bills_payment.FindAsync(id);
            if (billsPayment == null)
            {
                return NotFound();
            }
            ViewData["BillId"] = new SelectList(_context.bills, "BillId", "DateRange", billsPayment.BillId);
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email", billsPayment.ResidentId);
            return View(billsPayment);
        }

        // POST: Admin/BillsPayments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BillPaymentId,BillId,ResidentId,Amount,PaymentId,PaidAmount,Status,PaidOn")] BillsPayment billsPayment)
        {
            if (id != billsPayment.BillPaymentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(billsPayment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillsPaymentExists(billsPayment.BillPaymentId))
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
            ViewData["BillId"] = new SelectList(_context.bills, "BillId", "DateRange", billsPayment.BillId);
            ViewData["ResidentId"] = new SelectList(_context.residents, "ResidentId", "Email", billsPayment.ResidentId);
            return View(billsPayment);
        }

        // GET: Admin/BillsPayments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.bills_payment == null)
            {
                return NotFound();
            }

            var billsPayment = await _context.bills_payment
                .Include(b => b.Bill)
                .Include(b => b.Resident)
                .FirstOrDefaultAsync(m => m.BillPaymentId == id);
            if (billsPayment == null)
            {
                return NotFound();
            }

            return View(billsPayment);
        }

        // POST: Admin/BillsPayments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.bills_payment == null)
            {
                return Problem("Entity set 'ApplicationDbContext.bills_payment'  is null.");
            }
            var billsPayment = await _context.bills_payment.FindAsync(id);
            if (billsPayment != null)
            {
                _context.bills_payment.Remove(billsPayment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillsPaymentExists(int id)
        {
          return (_context.bills_payment?.Any(e => e.BillPaymentId == id)).GetValueOrDefault();
        }
    }
}
