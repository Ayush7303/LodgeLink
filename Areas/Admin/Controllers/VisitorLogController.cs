using LodgeLink.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LodgeLink.Areas.Admin.Controllers
{
    public class VisitorLogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VisitorLogController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

           
        }
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.visitors.Include(v => v.Property).Include(v => v.Property.Building);
            return View(await applicationDbContext.ToListAsync());
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

    }
}
