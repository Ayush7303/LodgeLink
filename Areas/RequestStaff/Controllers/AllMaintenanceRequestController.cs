using LodgeLink.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LodgeLink.Areas.RequestStaff.Controllers
{
    public class AllMaintenanceRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AllMaintenanceRequestController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult CompletedMaintenanceRequest()
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            var completedReq = _context.maintenanceRequests.Where(e => e.Status.Equals("Completed") && e.UserId == uid).Include(m => m.Property).Include(m => m.Resident).Include(m=>m.Property.Building).Include(m => m.User);
            return View(completedReq.ToList());
        }
        public IActionResult PendingMaintenanceRequest()
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            var pendingReq = _context.maintenanceRequests.Where(e => e.Status.Equals("Pending") && e.UserId == uid).Include(m => m.Property).Include(m => m.Resident).Include(m=>m.Property.Building).Include(m => m.User);
            return View(pendingReq);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PendingMaintenanceRequest(int requestId)
        {
            var requestToApprove = _context.maintenanceRequests.FirstOrDefault(e => e.RequestId==requestId);
            if (requestToApprove != null)
            {
                requestToApprove.CompletionDate=DateTime.Now;
                requestToApprove.Status = "Completed";
                
                TempData["success"] = "Maintenance Request Completed .";
                _context.SaveChanges();
            }

            return RedirectToAction("PendingMaintenanceRequest");
        }
    }
}
