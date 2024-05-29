using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace LodgeLink.Areas.Events.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult EventManagerProfile()
        {
            var uid = HttpContext.Session.GetInt32("UserId");

            var data = (from s in _context.users
                        where s.UserId == uid
                        select s).FirstOrDefault();
            return View(data);
        }
        [HttpPost]
        public IActionResult UpdateUserData([FromBody] User updatedUser)
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            // Retrieve the existing user from the database using the UserId or any unique identifier
            var existingUser = (from s in _context.users
                                where s.UserId == uid
                                select s).FirstOrDefault();

            // Update the properties
            existingUser.UserName = updatedUser.UserName;
            existingUser.FirstName = updatedUser.FirstName;
            existingUser.LastName = updatedUser.LastName;
            existingUser.Email = updatedUser.Email;

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
            var user = (from s in _context.users
                        where s.UserId == uid
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
                        file.CopyTo(filestream);
                    }
                    user.ProfilePicture = @"\images\User\" + filename;
                    HttpContext.Session.SetString("UserImage", user.ProfilePicture);

                }
                _context.SaveChanges();
                return RedirectToAction("EventManagerProfile", "Events", new { area = "Events" });

            }

            return BadRequest("Invalid file");
        }
        [HttpPost]
        public IActionResult UpdateInfo([FromBody] User updatedUser)
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            // Retrieve the existing user from the database using the UserId or any unique identifier
            var existingUser = (from s in _context.users
                                where s.UserId == uid
                                select s).FirstOrDefault();

            // Update the properties
            existingUser.Address = updatedUser.Address;
            existingUser.ContactNumber = updatedUser.ContactNumber;


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
            // Retrieve the existing user from the database using the UserId or any unique identifier
            var user = (from s in _context.users
                        where s.UserId == uid && s.Password == request.CurrentPassword
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
    }
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatNewPassword { get; set; }
    }
}
