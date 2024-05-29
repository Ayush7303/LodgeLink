using LodgeLink.Data;
using Microsoft.AspNetCore.Mvc;

namespace LodgeLink.Areas.Admin.Controllers
{
    public class FacilityRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilityRequestController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ApprovedFacilityRequests()
        {
            var approvedRequests = _context.bookingRequests.Where(r => r.Status == "Approved").ToList();
            return View(approvedRequests);
        }
        public IActionResult ApproveFacilityRequests()
        {
            var pendingRequests = _context.bookingRequests.Where(r => r.Status == "Pending").ToList();
            return View(pendingRequests);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveFacilityRequests(int requestId)
        {
            var requestToApprove = _context.bookingRequests.FirstOrDefault(r => r.RequestId == requestId);

            if (requestToApprove != null)
            {
                requestToApprove.Status = "Approved";
                requestToApprove.ApprovalDate = DateTime.Now;
                TempData["success"] = "Facility Booking Request Approved Successfully.";

                _context.SaveChanges();
            }

            return RedirectToAction("ApproveFacilityRequests");
        }
    }
}
