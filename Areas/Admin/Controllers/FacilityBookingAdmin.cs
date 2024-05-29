using LodgeLink.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace LodgeLink.Areas.Admin.Controllers
{
    public class FacilityBookingAdmin : Controller
    {
        private readonly ApplicationDbContext _context;
        public FacilityBookingAdmin(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var Request2 = _context.bookingRequests.Include(x => x.Resident).Include(x => x.Facility).ToList();

            var Request1 = _context.bookingRequests.GroupBy(ep => ep.FacilityId)
    .Select(group => group.First())
    .ToList();

            dynamic myModel = new ExpandoObject();
            myModel.Facility = Request1;
            myModel.FacilityBooking = Request2;
            return View(myModel);
        }
    }
}
