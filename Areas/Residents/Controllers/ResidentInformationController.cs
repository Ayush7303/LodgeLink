using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;

namespace LodgeLink.Areas.Residents.Controllers
{
    public class ResidentInformationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ResidentInformationController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            int? credentialIdFromSession = HttpContext.Session.GetInt32("CredId");
            int? propertyIdFromSession = HttpContext.Session.GetInt32("PropId");
            ViewBag.CID = credentialIdFromSession;
            ViewBag.PID=propertyIdFromSession;
            return View();
        }

        // POST: Residents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Resident resident, IFormFile? ProfileImage)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (ProfileImage != null)
            {
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(ProfileImage.FileName);
                string visitorPath = Path.Combine(wwwRootPath, @"images\Resident");
                using (var filestream = new FileStream(Path.Combine(visitorPath, filename), FileMode.Create))
                {
                    ProfileImage.CopyTo(filestream);
                }
                resident.ProfileImage = @"\images\Resident\" + filename;
            }

            HttpContext.Session.SetString("UserImage", resident.ProfileImage);
            resident.CreatedAt = DateTime.Now;
            resident.UpdatedAt = DateTime.Now;
            resident.IsActive = true;
            resident.Status = "Idle";
            if (ModelState.IsValid)
            {               
                _context.Add(resident);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("UserId", resident.ResidentId);
                return RedirectToAction("Index", "HomeResident");
            }
            ViewData["CredentialId"] = new SelectList(_context.credentials, "CredentialId", "CredentialId", resident.CredentialId);
            ViewData["PropertyId"] = new SelectList(_context.properties, "PropertyId", "PeopertyNumber", resident.PropertyId);
            return View(resident);
        }
    }
}
