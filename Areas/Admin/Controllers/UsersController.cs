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
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UsersController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.users.Include(u => u.Role);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var user = await _context.users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, IFormFile? ImageURL)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (ImageURL != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ImageURL.FileName);
                string visitorPath = Path.Combine(wwwRootPath, @"images\User");
                using (var filestream = new FileStream(Path.Combine(visitorPath, filename), FileMode.Create))
                {
                    ImageURL.CopyTo(filestream);
                }
                user.ProfilePicture = @"\images\User\" + filename;
            }
            if (ModelState.IsValid)
            {
                user.LastLogin = DateTime.Now;
                _context.Add(user);
                await _context.SaveChangesAsync();
                TempData["success"] = "User Created Successfully.";

                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var user = await _context.users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, IFormFile? ImageURL)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (ImageURL != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ImageURL.FileName);
                string productpath = Path.Combine(wwwRootPath, @"images\User");
                if (!string.IsNullOrEmpty(user.ProfilePicture))
                {
                    //old image delete
                    var oldpath = Path.Combine(wwwRootPath, user.ProfilePicture.TrimStart('\\'));
                    if (System.IO.File.Exists(oldpath))
                    {
                        System.IO.File.Delete(oldpath);
                    }
                }
                using (var filestream = new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                {
                    ImageURL.CopyTo(filestream);
                }
                user.ProfilePicture = @"\images\User\" + filename;
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["success"] = "User Updated Successfully.";

                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.roles, "RoleId", "RoleName", user.RoleId);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.users == null)
            {
                return NotFound();
            }

            var user = await _context.users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.users == null)
            {
                return Problem("Entity set 'ApplicationDbContext.users'  is null.");
            }
            var user = await _context.users.FindAsync(id);
            if (user != null)
            {
                _context.users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            TempData["success"] = "User Deleted Successfully.";

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.users?.Any(e => e.UserId == id)).GetValueOrDefault();
        }
    }
}
