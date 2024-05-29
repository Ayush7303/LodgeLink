using LodgeLink.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LodgeLink.Areas.Admin.Controllers
{
    public class ApproveRequestsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApproveRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult ApprovedRequests()
        {
            var approvedRequests = _context.architecturalRequests.Where(r => r.Status == "Approved").Include(r => r.Property).Include(r => r.Resident).Include(r => r.User);
            return View(approvedRequests.ToList());
        }
        public IActionResult RefusedArchitecturalRequest()
        {
            var approvedRequests = _context.architecturalRequests.Where(r => r.Status == "Refused").Include(r => r.Property).Include(r => r.Resident).Include(m => m.Property.Building).Include(r => r.User);
            return View(approvedRequests.ToList());
        }
        public IActionResult ApproveRequests()
        {
            var pendingRequests = _context.architecturalRequests.Where(r => r.Status == "Pending").Include(r => r.Property).Include(r => r.Resident).Include(r => r.User);
            return View(pendingRequests.ToList());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveRequest(int requestId)
        {
            var requestToApprove = _context.architecturalRequests.FirstOrDefault(r => r.RequestId == requestId);

            if (requestToApprove != null)
            {
                requestToApprove.Status = "Approved";
                requestToApprove.ApprovalDate = DateTime.Now;
                requestToApprove.UserId = HttpContext.Session.GetInt32("UserId"); // Set to the admin's username or ID
                TempData["success"] = "Architectural Request Approved Successfully.";
                _context.SaveChanges();
            }

            return RedirectToAction("ApproveRequests");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RefuseRequest(int requestId)
        {
            var requestToApprove = _context.architecturalRequests.FirstOrDefault(r => r.RequestId == requestId);

            if (requestToApprove != null)
            {
                requestToApprove.Status = "Refused";
                requestToApprove.ApprovalDate = DateTime.Now;
                var uid = HttpContext.Session.GetInt32("UserId");
                requestToApprove.UserId = uid; // Set to the admin's username or ID
                TempData["success"] = "Architectural Request Refused Successfully.";
                _context.SaveChanges();
            }

            return RedirectToAction("ApproveRequests");
        }
    }
}
