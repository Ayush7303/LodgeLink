using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Text;

namespace LodgeLink.Areas.Identity.Controllers
{
    public class IdentityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IdentityController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            List<Role> roles = _context.roles.ToList(); // Replace with your actual method to get roles

            // Convert the role data to SelectListItems
            List<SelectListItem> roleListItems = roles
                .Select(r => new SelectListItem { Value = r.RoleId.ToString(), Text = r.RoleName })
                .ToList();

            // Store role information in ViewBag or ViewData
            ViewBag.Roles = roleListItems;

            return View();

        }

        [HttpPost]
        public IActionResult Login(User user)
        {
            var role = user.RoleId;
            //int ret = checkCredentials(user);

           
                switch (role)
                {
                case 3: //Admin
                    List<User> userList = _context.users.Where(u => u.UserName.Equals(user.UserName) && u.Password.Equals(user.Password) && u.RoleId.Equals(role)).ToList();
                    if (userList.IsNullOrEmpty())
                    {
                        TempData["errorl"] = "Insert Correct Username And Password";
                        return RedirectToAction("Login", "Identity", new { area = "Identity" });
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                        {
                            var uid = (from s in _context.users
                                       where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                       select s.UserId).FirstOrDefault();
                            var LoggedInUser = (from s in _context.users
                                                where s.UserId == uid
                                                select s).FirstOrDefault();
                            LoggedInUser.LastLogin= DateTime.Now;
                            _context.users.Update(LoggedInUser);
                            _context.SaveChanges();
                            var uimage = (from s in _context.users
                                          where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                          select s.ProfilePicture).FirstOrDefault();
                            HttpContext.Session.SetString("UserImage", uimage);
                            HttpContext.Session.SetInt32("UserId", uid);
                            HttpContext.Session.SetString("Username", user.UserName);
                            HttpContext.Session.SetString("Password", user.Password);
                        }
                        return RedirectToAction("Index", "Home", new { area = "Admin" });

                    }
                    break;
                case 4: //Resident
                    List<Credentials> credList = _context.credentials.Where(u => u.Username.Equals(user.UserName) && u.Password.Equals(user.Password)).ToList();
                    if (credList.IsNullOrEmpty())
                    {
                        TempData["errorl"] = "Insert Correct Username And Password";

                        return RedirectToAction("Login", "Identity", new { area = "Identity" });

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                        {
                            Credentials userCredentials = credList.First();

                            Resident existingResident = _context.residents.FirstOrDefault(r => r.CredentialId == userCredentials.CredentialId);

                            if (existingResident!=null)
                            {
                                var cid = (from s in _context.credentials
                                           where s.Username.Equals(user.UserName) && s.Password.Equals(user.Password)
                                           select s.CredentialId).FirstOrDefault();
                                var uid = (from s in _context.residents
                                           where s.CredentialId == cid
                                           select s.ResidentId).FirstOrDefault();
                                var uimage = (from s in _context.residents
                                              where s.CredentialId == cid
                                              select s.ProfileImage).FirstOrDefault();

                                HttpContext.Session.SetInt32("UserId", uid);
                                HttpContext.Session.SetString("UserImage", uimage);
                                HttpContext.Session.SetString("Username", user.UserName);
                                HttpContext.Session.SetString("Password", user.Password);
                                HttpContext.Session.SetInt32("ResId", existingResident.ResidentId);
                                return RedirectToAction("Index", "HomeResident", new { area = "Residents" });
                            }
                            else
                            {
                                

                                HttpContext.Session.SetString("Username", user.UserName);
                                HttpContext.Session.SetString("Password", user.Password);
                                HttpContext.Session.SetInt32("CredId", userCredentials.CredentialId);
                                HttpContext.Session.SetInt32("PropId", userCredentials.PropertyId);
                                return RedirectToAction("Create", "ResidentInformation", new { area = "Residents" });
                            }

                        }

                    }
                    
                        break;

                case 5: //Security Personnel
                    List<User> securityList = _context.users.Where(u => u.UserName.Equals(user.UserName) && u.Password.Equals(user.Password) && u.RoleId.Equals(role)).ToList();
                    if (securityList.IsNullOrEmpty())
                    {
                        TempData["errorl"] = "Insert Correct Username And Password";

                        return RedirectToAction("Login", "Identity", new { area = "Identity" });
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                        {
                            var uid = (from s in _context.users
                                      where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                      select s.UserId).FirstOrDefault();
                            var LoggedInUser = (from s in _context.users
                                                where s.UserId == uid
                                                select s).FirstOrDefault();
                            LoggedInUser.LastLogin = DateTime.Now;
                            _context.users.Update(LoggedInUser);
                            _context.SaveChanges();
                            var uimage = (from s in _context.users
                                       where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                       select s.ProfilePicture).FirstOrDefault();
                            HttpContext.Session.SetString("UserImage", uimage);
                            HttpContext.Session.SetInt32("UserId",uid);
                            HttpContext.Session.SetString("Username", user.UserName);
                            HttpContext.Session.SetString("Password", user.Password);
                        }
                        return RedirectToAction("Create", "Visitors", new { area = "Security_Personnel" });

                    }
                    break;
                case 6: //Event Manager
                    List<User> eventList = _context.users.Where(u => u.UserName.Equals(user.UserName) && u.Password.Equals(user.Password) && u.RoleId.Equals(role)).ToList();
                    if (eventList.IsNullOrEmpty())
                    {
                        TempData["error"] = "Insert Correct Username And Password";

                        return RedirectToAction("Login", "Identity", new { area = "Identity" });
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                        {
                            var uid = (from s in _context.users
                                       where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                       select s.UserId).FirstOrDefault();
                            var LoggedInUser = (from s in _context.users
                                                where s.UserId == uid
                                                select s).FirstOrDefault();
                            LoggedInUser.LastLogin = DateTime.Now;
                            _context.users.Update(LoggedInUser);
                            _context.SaveChanges();
                            var uimage = (from s in _context.users
                                          where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                          select s.ProfilePicture).FirstOrDefault();
                            HttpContext.Session.SetString("UserImage", uimage);
                            HttpContext.Session.SetInt32("UserId", uid);
                            HttpContext.Session.SetString("Username", user.UserName);
                            HttpContext.Session.SetString("Password", user.Password);
                        }
                        return RedirectToAction("Index", "EventManage", new { area = "Events" });

                    }
                    break;

                //case 3:
                //    return RedirectToAction("Index", "Event");
                //    break;
                case 7: //Architecutal Management
                    List<User> reqmanageList = _context.users.Where(u => u.UserName.Equals(user.UserName) && u.Password.Equals(user.Password) && u.RoleId.Equals(role)).ToList();
                    if (reqmanageList.IsNullOrEmpty())
                    {
                        TempData["error"] = "Insert Correct Username And Password";

                        return RedirectToAction("Login", "Identity", new { area = "Identity" });
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                        {
                            var uid = (from s in _context.users
                                       where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                       select s.UserId).FirstOrDefault();
                            var LoggedInUser = (from s in _context.users
                                                where s.UserId == uid
                                                select s).FirstOrDefault();
                            LoggedInUser.LastLogin = DateTime.Now;
                            _context.users.Update(LoggedInUser);
                            _context.SaveChanges();
                            var uimage = (from s in _context.users
                                          where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                          select s.ProfilePicture).FirstOrDefault();
                            HttpContext.Session.SetString("UserImage", uimage);
                            HttpContext.Session.SetInt32("UserId", uid);
                            HttpContext.Session.SetString("Username", user.UserName);
                            HttpContext.Session.SetString("Password", user.Password);
                        }
                        return RedirectToAction("ApproveArchitecturalRequest", "ArchitecturalManagement", new { area = "RequestManager" });

                    }
                    break;
                case 8: //Request Staff
                    List<User> reqStaffList = _context.users.Where(u => u.UserName.Equals(user.UserName) && u.Password.Equals(user.Password) && u.RoleId.Equals(role)).ToList();
                    if (reqStaffList.IsNullOrEmpty())
                    {
                        TempData["error"] = "Insert Correct Username And Password";

                        return RedirectToAction("Login", "Identity", new { area = "Identity" });
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                        {
                            var uid = (from s in _context.users
                                       where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                       select s.UserId).FirstOrDefault();
                            var LoggedInUser = (from s in _context.users
                                                where s.UserId == uid
                                                select s).FirstOrDefault();
                            LoggedInUser.LastLogin = DateTime.Now;
                            _context.users.Update(LoggedInUser);
                            _context.SaveChanges();
                            var uimage = (from s in _context.users
                                          where s.UserName.Equals(user.UserName) && s.Password.Equals(user.Password)
                                          select s.ProfilePicture).FirstOrDefault();
                            HttpContext.Session.SetString("UserImage", uimage);
                            HttpContext.Session.SetInt32("UserId", uid);
                            HttpContext.Session.SetString("Username", user.UserName);
                            HttpContext.Session.SetString("Password", user.Password);
                        }
                        return RedirectToAction("PendingMaintenanceRequest", "AllMaintenanceRequest", new { area = "RequestStaff" });

                    }
                    break;

                default:
                    return RedirectToAction("Login", "Identity", new { area = "Identity" });

                    break;
                }
            return View();
        }
        public IActionResult ForgotPassword()
        {
            ViewData["FormType"] = 0;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(string email, string otp, string newPassword)
        {
            if (!string.IsNullOrEmpty(email) && string.IsNullOrEmpty(otp) && string.IsNullOrEmpty(newPassword))
            {
                try
                {
                    var isfound = _context.residents.Where(r => r.Email.Equals(email)).Count();
                    if (isfound > 0)
                    {
                        string URL = "http://localhost:5205/EmailService";
                        Random rnd = new Random();
                        int newotp = rnd.Next();
                        EmailData emaildata = new EmailData();
                        emaildata.To = email;
                        emaildata.Subject = "OTP For Changing Password";

                        emaildata.Body = "Your OTP : " + newotp;

                        HttpContext.Session.SetString("OTP", Convert.ToString(newotp));

                        using (HttpClient http = new HttpClient())
                        {
                            var newData = JsonConvert.SerializeObject(emaildata);
                            HttpContent content = new StringContent(newData, Encoding.UTF8, "application/json");
                            using (var res = await http.PostAsync(URL, content))
                            {
                                var apires = await res.Content.ReadAsStringAsync();
                            }
                        }
                        ViewData["FormType"] = 1;
                        ViewData["Email"] = email;
                    }
                    else
                    {
                        ViewData["FormType"] = 0;
                        ViewData["EmailError"] = "Email Not Found!!";
                    }
                }
                catch
                {
                    ViewData["FormType"] = 0;
                }

            }
            else if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(otp) && string.IsNullOrEmpty(newPassword))
            {
                ViewData["FormType"] = 2;
                ViewData["Email"] = email;
            }
            else if (!string.IsNullOrEmpty(email) && string.IsNullOrEmpty(otp) && !string.IsNullOrEmpty(newPassword))
            {
                try
                {
                    var pid = (from r in _context.residents
                               where r.Email.Equals(email)
                               select r.PropertyId).FirstOrDefault();

                    var resident = (from c in _context.credentials
                                    where c.PropertyId.Equals(pid)
                                    select c).FirstOrDefault();

                    resident.Password = newPassword;
                    _context.credentials.Update(resident);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "Identity", new { area = "Identity" });
                }
                catch
                {
                    return RedirectToAction("ForgotPassword", "Identity", new { area = "Identity" });
                }
            }
            return View();
        }

    


    public IActionResult Logout()
        {
    
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Identity", new { area = "Identity" });
        }
    }
}
