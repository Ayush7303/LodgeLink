using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Razorpay.Api;
using System.Text;

namespace LodgeLink.Areas.Residents.Controllers
{
    public class FacilityResidentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private string _key = "rzp_test_ZzameYlspguJao";
        private string _secret = "VSh52YduJ7c6QyXtsX57nzF7";

        public FacilityResidentController(ApplicationDbContext db)
        {
            _db = db;
        }
        // GET: FacilityResidentController
        public ActionResult Index()
        {
            return View(_db.facilities.ToList());
        }

        // GET: FacilityResidentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        private string Payment(FacilityPaymentModel payment)
        {
            try
            {

                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", Convert.ToDecimal(payment.Amount) * 100);
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
        // GET: FacilityResidentController/Create
        public ActionResult Create(int id)
        {
            var facility = _db.facilities.FirstOrDefault(f => f.FacilityId == id);
            return View(facility);
        }

        public ActionResult PaymentViewFacility(int id, DateTime StartDate, DateTime EndDate)
        {
            var resid = HttpContext.Session.GetInt32("ResId");
            var facility = _db.facilities.FirstOrDefault(f => f.FacilityId == id);
            if (id.Equals(1)|| id.Equals(5) || id.Equals(7))
            {
                var booking = _db.bookingRequests.Where(b => b.FacilityId == id).Where(b=>b.StartDate <= StartDate && b.EndDate >= StartDate || b.StartDate >= StartDate && b.EndDate <= EndDate || b.StartDate >= StartDate && b.EndDate >= EndDate);
                if (booking.Count() > 0)
                {
                    TempData["FacilityError"] = "The Amenity already booked between or around Time-range. Please Choose other Date, Else if urgent then Directly Contact us.";
                    return RedirectToAction("Create", new {id = id});
                }
                else
                {
                    FacilityPaymentModel payment = new FacilityPaymentModel
                    {
                        Name = "Jeshan Patel",
                        Email = "jeshanpatel123@gmail.com",
                        Contact = "7069170879",
                        Amount = (decimal)facility.BookingFee * 100,
                        Amount_Rs = (decimal)facility.BookingFee,
                        Currency = "INR",
                        Payment_Capture = 1,
                        Notes = "Paid!!",
                        OrderId = "",
                        Key = "",
                        ImageURL = "/images/Logo.png",
                        FacilityId = facility.FacilityId,
                        FacilityName = facility.FacilityName,
                        Status = "Booked",
                        ResidentId = resid,
                        StartDate = StartDate,
                        EndDate = EndDate
                    };

                    var orderid = Payment(payment);
                    payment.OrderId = orderid;
                    payment.Key = _key;
                    return View(payment);
                }
            }
            else
            {

                FacilityPaymentModel payment = new FacilityPaymentModel
                {
                    Name = "Jeshan Patel",
                    Email = "jeshanpatel123@gmail.com",
                    Contact = "7069170879",
                    Amount = (decimal)facility.BookingFee * 100,
                    Amount_Rs = (decimal)facility.BookingFee,
                    Currency = "INR",
                    Payment_Capture = 1,
                    Notes = "Paid!!",
                    OrderId = "",
                    Key = "",
                    ImageURL = "/images/Logo.png",
                    FacilityId = facility.FacilityId,
                    FacilityName = facility.FacilityName,
                    Status = "Booked",
                    ResidentId = resid,
                    StartDate = StartDate,
                    EndDate = EndDate
                };

                var orderid = Payment(payment);
                payment.OrderId = orderid;
                payment.Key = _key;
                return View(payment);
            }
        }

        // POST: FacilityResidentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookingRequest request)
        {
            request.RequestDate = DateTime.Now;
            request.PaidAmount = request.PaidAmount;
            request.Status = "Booked";
            request.NotificationSent = false;
            string URL = "http://localhost:5205/EmailService";
            try
            {
                _db.bookingRequests.Add(request);
                _db.SaveChanges();
                TempData["success"] = "Amenity Booked Successfully.";

                var resid = HttpContext.Session.GetInt32("ResId");
                var res = _db.residents.FirstOrDefault(r => r.ResidentId == resid);
                var emenity = _db.facilities.FirstOrDefault(f => f.FacilityId == request.FacilityId);

                EmailData email = new EmailData();
                email.To = res.Email;
                email.Subject = "Amenity Booking Notification";
                email.Body = "Congratulation " + res.FirstName + " " + res.LastName + "!! You have Booked Amenity " + emenity.FacilityName + ". Hope You enjoy it. Have a Nice Day!!";

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
                return RedirectToAction("Create");
            }
        }

        // GET: FacilityResidentController/Edit/5
        public ActionResult BookedEmenities()
        {
            var resid = HttpContext.Session.GetInt32("ResId");
            return View(_db.bookingRequests.Where(b=>b.ResidentId == resid).Include(b=>b.Facility));
        }
    }
    public class FacilityPaymentModel
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public decimal Amount { get; set; }
        public decimal Amount_Rs { get; set; }
        public string Currency { get; set; }
        public int Payment_Capture { get; set; }
        public string Notes { get; set; }
        public string OrderId { get; set; }
        public string ImageURL { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int? ResidentId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
    }
}
