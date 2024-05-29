using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;
using System.Security.Policy;
using System.Text;

namespace LodgeLink.Areas.Residents.Controllers
{
    public class RequestController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnv;
        public RequestController(ApplicationDbContext db, IWebHostEnvironment webHostEnv)
        {
            _db = db;
            _webHostEnv = webHostEnv;
        }

        public IActionResult Index()
        {
            var resident = _db.credentials.FirstOrDefault(u => u.Username == HttpContext.Session.GetString("Username") && u.Password == HttpContext.Session.GetString("Password"));
            var resid = _db.residents.FirstOrDefault(r => r.CredentialId.Equals(resident.CredentialId)).ResidentId;
            var Request1 = _db.architecturalRequests.Where(a => a.ResidentId.Equals(resid)).ToList();
            var Request2 = _db.maintenanceRequests.Where(a => a.ResidentId.Equals(resid)).ToList();
            dynamic myModel = new ExpandoObject();
            myModel.Architecture = Request1;
            myModel.Maintanance = Request2;

            return View(myModel);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Requests request, IFormFile? attachments)
        {
            string URL = "http://localhost:5205/EmailService";
            var resident = _db.credentials.FirstOrDefault(u => u.Username == HttpContext.Session.GetString("Username") && u.Password == HttpContext.Session.GetString("Password"));
            if (ModelState.IsValid)
            {
                //string wwwRootPath = _webHostEnv.WebRootPath;
                if (attachments != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(attachments.FileName);
                    string albumPath = Path.Combine(_webHostEnv.WebRootPath, @"images");

                    using (var fileStream = new FileStream(Path.Combine(albumPath, filename), FileMode.Create))
                    {
                        attachments.CopyTo(fileStream);
                    }
                    request.Attachments = filename;

                    if (request.Category == "1")
                    {
                        MaintenanceRequest maintenance = new MaintenanceRequest();
                       
                        maintenance.Description = request.Description;
                        maintenance.Notes = request.Notes;
                        maintenance.ResidentId = _db.residents.FirstOrDefault(r => r.CredentialId == resident.CredentialId).ResidentId;
                        maintenance.RequestDate = DateTime.Now;
                        maintenance.Attachments = request.Attachments;
                        maintenance.PropertyId = resident.PropertyId;
                        maintenance.Status = "Pending";
                        _db.maintenanceRequests.Add(maintenance);
                        _db.SaveChanges();

                        TempData["success"] = "Request Sent Successfully.";

                        var resid = HttpContext.Session.GetInt32("ResId");
                        var res = _db.residents.FirstOrDefault(r => r.ResidentId == resid);

                        EmailData email = new EmailData();
                        email.To = res.Email;
                        email.Subject = "Maintenence Request Notification";
                        email.Body = "Congratulation " + res.FirstName + " " + res.LastName + "!! Your Request for " + request.Description + " sent successfully. We'll approve your request and try to solve your issue as soon as possible. Have a Nice Day!!";

                        using (HttpClient http = new HttpClient())
                        {
                            var newData = JsonConvert.SerializeObject(email);
                            HttpContent content = new StringContent(newData, Encoding.UTF8, "application/json");
                            using (var resp = await http.PostAsync(URL, content))
                            {
                                var apires = await resp.Content.ReadAsStringAsync();
                            }
                        }
                    }
                }
                else
                {
                    if (request.Category == "0")
                    {
                        ArchitecturalRequest arc = new ArchitecturalRequest();
                        arc.Description = request.Description;
                        arc.CompletionDate = request.CompletionDate;
                        arc.Notes = request.Notes;
                        arc.ResidentId = _db.residents.FirstOrDefault(r => r.CredentialId == resident.CredentialId).ResidentId;
                        arc.RequestDate = DateTime.Now;
                        arc.PropertyId = resident.PropertyId;
                        arc.Status = "Pending";
                        _db.architecturalRequests.Add(arc);
                        _db.SaveChanges();
                        TempData["success"] = "Request Sent Successfully.";

                        var resid = HttpContext.Session.GetInt32("ResId");
                        var res = _db.residents.FirstOrDefault(r => r.ResidentId == resid);

                        EmailData email = new EmailData();
                        email.To = res.Email;
                        email.Subject = "Architecture Request Notification";
                        email.Body = "Congratulation " + res.FirstName + " " + res.LastName + "!! Your Request for " + request.Description + " sent successfully. We'll approve your request and try to solve your issue before "+request.CompletionDate.Value.ToShortDateString()+" as possible. Please wait for our approval before taking furthur decisions. Have a Nice Day!!";

                        using (HttpClient http = new HttpClient())
                        {
                            var newData = JsonConvert.SerializeObject(email);
                            HttpContent content = new StringContent(newData, Encoding.UTF8, "application/json");
                            using (var resp = await http.PostAsync(URL, content))
                            {
                                var apires = await resp.Content.ReadAsStringAsync();
                            }
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
    }
}

