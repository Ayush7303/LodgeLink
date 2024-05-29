using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Razorpay.Api;
using System.Dynamic;
using System.Globalization;
using System.Text;

namespace LodgeLink.Areas.Residents.Controllers
{
    public class EventBookingController : Controller
    {
        private readonly ApplicationDbContext _db;
        private string _key = "rzp_test_ZzameYlspguJao";
        private string _secret = "VSh52YduJ7c6QyXtsX57nzF7";
        string URL = "http://localhost:5205/EmailService";

        public EventBookingController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: EventController1
        public ActionResult Index()
        {
            var residentId = HttpContext.Session.GetInt32("UserId"); // Implement your logic to get resident ID

            var unbookedEvents = from e in _db.events
                                 where !_db.eventParticipants.Any(ep => ep.ResidentId == residentId && ep.EventId == e.EventId)
                                 select e;

            ViewData["dataUnbooked"] = unbookedEvents.ToList();

            var data = _db.events.ToList();
            ViewData["data"] = data;
            
            return View();
        }

        // GET: EventController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: EventController1/Create
        public ActionResult EventParticipation(int id)
        {
            var resid = HttpContext.Session.GetInt32("ResId");
            var events = _db.events.FirstOrDefault(e => e.EventId == id);

            EventPaymentModel payment = new EventPaymentModel
            {
                Name = "Jeshan Patel",
                Email = "jeshanpatel123@gmail.com",
                Contact = "7069170879",
                Amount = (decimal)events.Fees,
                Currency = "INR",
                Payment_Capture = 1,
                Notes = "Paid!!",
                OrderId = "",
                Key = "",
                ImageURL = "/images/Logo.png",
                EventId = events.EventId,
                Subject = events.Subject,
                Status = "Participated",
                ResidentId = resid
            };

            var orderid = Payment(payment);
            payment.OrderId = orderid;
            payment.Key = _key;
                
            return View(payment);
        }
        private string Payment(EventPaymentModel payment)
        {
            try
            {

                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", Convert.ToDecimal(payment.Amount) *100);
                options.Add("currency", payment.Currency);
                options.Add("payment_capture", payment.Payment_Capture);
                options.Add("notes", payment.Notes);

                Order order = client.Order.Create(options);
                var orderid = order.Attributes["id"].ToString();

                return orderid;
            }
            catch
            {
                return null;
            }
        }

        public ActionResult PaymentViewEvent(EventPaymentModel events)
        {
            var resid = HttpContext.Session.GetInt32("ResId");

            EventPaymentModel payment = new EventPaymentModel
            {
                Name = "Jeshan Patel",
                Email = "jeshanpatel123@gmail.com",
                Contact = "7069170879",
                Amount = (decimal)events.Amount*100,
                Currency = "INR",
                Payment_Capture = 1,
                Notes = "Paid!!",
                OrderId = "",
                Key = "",
                ImageURL = "/images/Logo.png",
                EventId = events.EventId,
                Subject = events.Subject,
                Status = "Participated",
                Members = events.Members,
                ResidentId = resid
            };

            var orderid = Payment(payment);
            payment.OrderId = orderid;
            payment.Key = _key;

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EventParticipate(EventParticipant participant)
        {
            try
            {
                participant.PaidAmount = participant.PaidAmount / 100;
                participant.RegistrationDate = DateTime.Now;
                _db.eventParticipants.Add(participant);
                _db.SaveChanges();

                TempData["success"] = "Event Participated Successfully.";

                var resid = HttpContext.Session.GetInt32("ResId");
                var res = _db.residents.FirstOrDefault(r => r.ResidentId == resid);
                var events = _db.events.FirstOrDefault(e => e.EventId == participant.EventId);
                EmailData email = new EmailData();
                email.To = res.Email;
                email.Subject = "Event Participation Notification";
                email.Body = "Congratulation " + res.FirstName + " " + res.LastName + "!! You have Participated in " + events.Subject + ". Which helds on " + events.Start.Value.ToShortDateString() + ". Hope You enjoy it. Have a Nice Day!!";

                using (HttpClient http = new HttpClient())
                {
                    var newData = JsonConvert.SerializeObject(email);
                    HttpContent content = new StringContent(newData, Encoding.UTF8, "application/json");
                    using (var resp = await http.PostAsync(URL, content))
                    {
                        var apires = await resp.Content.ReadAsStringAsync();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EventController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View(_db.events.FirstOrDefault(e => e.EventId == id));
        }

        public ActionResult MyEvents()
        {
            var resid = HttpContext.Session.GetInt32("UserId");
            return View(_db.eventParticipants.Where(e => e.ResidentId.Equals(resid)).Include(e => e.Event));
        }
        // POST: EventController1/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                var events = _db.events.FirstOrDefault(e => e.EventId == id);
                _db.events.Update(events);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var participant = _db.eventParticipants.FirstOrDefault(e => e.ParticipantId == id);
                _db.eventParticipants.Remove(participant);
                _db.SaveChanges();

                TempData["success"] = "Event Cancelled Successfully.";

                var resid = HttpContext.Session.GetInt32("ResId");
                var res = _db.residents.FirstOrDefault(r => r.ResidentId == resid);
                var events = _db.events.FirstOrDefault(e => e.EventId == participant.EventId);
                var returnAmount = participant.PaidAmount - participant.PaidAmount * 0.75;

                EmailData email = new EmailData();
                email.To = res.Email;
                email.Subject = "Event Participation Cancellation Notification";
                email.Body = res.FirstName + " " + res.LastName + "!! Your Participation in " + events.Subject + " Event Successfully Cancelled with return of " + returnAmount + " Amount. Have a Nice Day!!";

                using (HttpClient http = new HttpClient())
                {
                    var newData = JsonConvert.SerializeObject(email);
                    HttpContent content = new StringContent(newData, Encoding.UTF8, "application/json");
                    using (var resp = await http.PostAsync(URL, content))
                    {
                        var apires = await resp.Content.ReadAsStringAsync();
                    }
                }

                return RedirectToAction("MyEvents");
            }
            catch
            {
                return RedirectToAction(nameof(MyEvents));
            }
        }
    }

    public class EventPaymentModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int Payment_Capture { get; set; }
        public string Notes { get; set; }
        public string OrderId { get; set; }
        public string ImageURL { get; set; }
        public int EventId { get; set; }
        public string Subject { get; set; }
        public int? ResidentId { get; set; }
        public int? Members { get; set; }
        public string Status { get; set; }
    }
}
