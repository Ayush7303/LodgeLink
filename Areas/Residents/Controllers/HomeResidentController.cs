using LodgeLink.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using LodgeLink.Data;
using System.Dynamic;
using Microsoft.EntityFrameworkCore;
using LodgeLink.Areas.Residents.Services;

namespace LodgeLink.Areas.Residents.Controllers
{
    public class HomeResidentController : Controller
    {
        private readonly ILogger<HomeResidentController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public readonly IAnnouncementService _announcementService;

        public HomeResidentController(ILogger<HomeResidentController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, IAnnouncementService announcementService)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _announcementService = announcementService;
        }

        public IActionResult Index()
        {
            var events = _context.events.Where(e => e.Start > DateTime.Today).ToList();
            var services = _context.facilities.ToList();
            int houses = _context.properties.Count();
            int owners = _context.residents.Count();
            int servicecount = _context.facilities.Count();
            int requests = _context.architecturalRequests.Where(r => r.Status == "Completed").Count() + _context.maintenanceRequests.Where(r => r.Status == "Completed").Count();

            ViewData["houses"] = houses;
            ViewData["owners"] = owners;
            ViewData["services"] = servicecount;
            ViewData["requests"] = requests;
            ViewData["EventList"] = events;
            dynamic MyObject = new ExpandoObject();

            MyObject.Events = events;
            MyObject.Services = services;

            return View(MyObject);
        }

        public JsonResult GetAnnouncements()
        {
            return Json(_context.announcements.ToList());
        }

        public IActionResult ResidentProfile()
        {
            var uid = HttpContext.Session.GetInt32("UserId");

            var data = (from s in _context.residents
                        where s.ResidentId == uid
                        select s).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public IActionResult UpdateUserData([FromBody] Resident updatedUser)
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            // Retrieve the existing user from the database using the UserId or any unique identifier
            var existingUser = (from s in _context.residents
                                where s.ResidentId == uid
                                select s).FirstOrDefault();

            // Update the properties
            existingUser.ContactNumber = updatedUser.ContactNumber;
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;
            existingUser.UpdatedAt = DateTime.Now;
            // Save changes to the database
            _context.SaveChanges();

            return Json(new { success = true, message = "User Information Updated." });
            // Return a response

        }
        [HttpPost]
        public IActionResult UpdateProfilePicture(IFormFile file)
        {
            // Assuming you're using ASP.NET Core Identity
            var uid = HttpContext.Session.GetInt32("UserId");
            // Retrieve the existing user from the database using the UserId or any unique identifier
            var user = (from s in _context.residents
                        where s.ResidentId == uid
                        select s).FirstOrDefault();

            if (user == null)
            {
                return NotFound("User not found");
            }

            // Handle file upload
            if (file != null && file.Length > 0)
            {
                // Save the file to a location (you may want to save it in a specific folder or a cloud storage service)
                // For simplicity, let's assume the file is saved in wwwroot/images
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productpath = Path.Combine(wwwRootPath, @"images\Resident");
                    if (!string.IsNullOrEmpty(user.ProfileImage))
                    {
                        //old image delete
                        var oldpath = Path.Combine(wwwRootPath, user.ProfileImage.TrimStart('\\'));
                        if (System.IO.File.Exists(oldpath))
                        {
                            System.IO.File.Delete(oldpath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(productpath, filename), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    user.ProfileImage = @"\images\Resident\" + filename;
                    HttpContext.Session.SetString("UserImage", user.ProfileImage);

                }
                user.UpdatedAt = DateTime.Now;
                _context.SaveChanges();
                return RedirectToAction("ResidentProfile", "HomeResident", new { area = "Residents" });

            }

            return BadRequest("Invalid file");
        }
        [HttpPost]
        public IActionResult UpdateInfo([FromBody] Resident updatedUser)
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            // Retrieve the existing user from the database using the UserId or any unique identifier
            var existingUser = (from s in _context.residents
                                where s.ResidentId == uid
                                select s).FirstOrDefault();

            // Update the properties
            existingUser.DateOfBirth = updatedUser.DateOfBirth;
            existingUser.DateOfMoveIn = updatedUser.DateOfMoveIn;
            existingUser.EmergencyContactNumber = updatedUser.EmergencyContactNumber;

            existingUser.UpdatedAt=DateTime.Now;
            // Save changes to the database
            _context.SaveChanges();

            return Json(new { success = true, message = "User Information Updated." });
            // Return a response

        }
        [HttpPost]
        public IActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            // Retrieve the current user (you may need to adjust this based on your authentication mechanism)
            var uid = HttpContext.Session.GetInt32("UserId");
            var cid=(from s in _context.residents
                     where s.ResidentId==uid
                     select s.CredentialId).FirstOrDefault();
            // Retrieve the existing user from the database using the UserId or any unique identifier
            var user = (from s in _context.credentials
                        where s.CredentialId == cid && s.Password == request.CurrentPassword
                        select s).FirstOrDefault();

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Verify the current password (you may need to adjust this based on your authentication mechanism)


            // Update the password
            user.Password = request.NewPassword;
            _context.SaveChanges();

            return Ok(new { message = "Password updated successfully" });
        }
        public IActionResult Dashboard()
        {
            //var user = _context.credentials.FirstOrDefault(u => u.Username == HttpContext.Session.GetString("Username") && u.Password == HttpContext.Session.GetString("Password"));
            //var resid = _context.residents.FirstOrDefault(r => r.CredentialId.Equals(user.CredentialId)).ResidentId;
            //var resident = _context.residents.Where(bp => bp.ResidentId.Equals(resid)).Include(b => b.Credentials).Include(p => p.Property);
            //var payment = _context.bills_payment.Where(bp => bp.ResidentId.Equals(resid)).Include(p => p.Bill);
            //var maintanence = _context.maintenanceRequests.Where(bp => bp.ResidentId.Equals(resid));
            //var guest = _context.visitors.Where(bp => bp.PropertyId.Equals(_context.credentials.FirstOrDefault(c => c.PropertyId.Equals(user.PropertyId)).PropertyId));
            var uid = HttpContext.Session.GetInt32("UserId");
            var pid = (from r in _context.residents
                       where r.ResidentId == uid
                       select r.PropertyId).FirstOrDefault();
            var bid = (from p in _context.properties
                       where p.PropertyId == pid
                       select p.BuildingId).FirstOrDefault();
            var bname = (from b in _context.buildings
                         where b.BuildingId == bid
                         select b.BuildingName).FirstOrDefault();
            var pname = (from p in _context.properties
                         where p.PropertyId == pid
                         select p.PeopertyNumber).FirstOrDefault();
            var ptype = (from p in _context.properties
                         where p.PropertyId == pid
                         select p.PropertyType).FirstOrDefault();

            ViewBag.BuildingName = bname;
            ViewBag.PropertyNumber = pname;
            ViewBag.PropertyType = ptype;


            var visitorList = (from v in _context.visitors
                               where v.PropertyId == pid
                               select v).ToList();
            var visitorCount = visitorList.Count();
            ViewBag.VisitorCount = visitorCount;
            ViewData["VisitorList"]=visitorList;

            var PendingBill = (from b in _context.bills_payment
                               where b.ResidentId == uid && b.Status==false
                               select b).Include(x=>x.Bill).ToList();
            ViewData["PendingBills"] = PendingBill;


            var eventList = (from ev in _context.events
                             where ev.Fees == 0
                             select ev).ToList();

            var participatedEvents = (from ep in _context.eventParticipants
                                      where ep.ResidentId == uid
                                      select ep.Event).ToList();

            var combinedList = participatedEvents.Union(eventList).OrderBy(x=>x.Start).ToList();
           
            var eventCount = combinedList.Count(x=>x.Start>=DateTime.Now);
            ViewBag.EventCount = eventCount;
            ViewData["EventParticipant"]=combinedList;

            var archList = (from ar in _context.architecturalRequests
                            where ar.ResidentId == uid
                            select ar).Include(x=>x.User).ToList();
            ViewData["ArchRequest"] = archList;
            var archReqCount = archList.Count();
            ViewBag.archReqCount = archReqCount;


            var mainList = (from mr in _context.maintenanceRequests
                            where mr.ResidentId == uid
                            select mr).Include(x=>x.User).ToList();
            ViewData["MainRequest"] = mainList;
            var mainReqCount = mainList.Count();
            ViewBag.MainRequestCount = mainReqCount;

            var facbooking = (from f in _context.bookingRequests
                              where f.ResidentId == uid
                              select f).Include(x => x.Facility).ToList();
            ViewData["FacBooking"] = facbooking;
            var facbookingCount = facbooking.Count();
            ViewBag.FacBookingCount = facbookingCount;

            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatNewPassword { get; set; }
    }
}