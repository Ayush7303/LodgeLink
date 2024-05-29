using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Razorpay.Api;
using System.Security.Policy;
using System.Text;
using System.Timers;

namespace LodgeLink.Areas.Residents.Controllers
{
    public class BillsResidentController : Controller
    {
        private readonly ApplicationDbContext _db;
        private string _key = "rzp_test_ZzameYlspguJao";
        private string _secret = "VSh52YduJ7c6QyXtsX57nzF7";
        //private readonly int resid = 0;

        public BillsResidentController(ApplicationDbContext db)
        {
            _db = db;
            //resid = (int)HttpContext.Session.GetInt32("ResId");
        }
        // GET: BillsController
        public ActionResult Index()
        {
            var uid = HttpContext.Session.GetInt32("UserId");
            var resid = HttpContext.Session.GetInt32("ResId");
            var billsofuser = _db.bills_payment.Include(b=>b.Resident).Include(b=>b.Bill).Where(b => b.ResidentId.Equals(resid)).ToList() ;

            var propid = _db.residents.FirstOrDefault(r => r.ResidentId.Equals(uid)).PropertyId;
            var bid = _db.properties.FirstOrDefault(p => p.PropertyId.Equals(propid)).BuildingId;
            var size = _db.properties.FirstOrDefault(p => p.PropertyId.Equals(propid)).PropertySize;
            var units = _db.buildings.FirstOrDefault(b => b.BuildingId.Equals(bid)).TotalUnits;
            var totalcost = 500000;

            var per_square_foot = totalcost / size;
            per_square_foot = per_square_foot / units;

            CalculateLateFees((int)resid);
            int count = 1;
            foreach (var bill in billsofuser)
            {
                ViewData["LateCharge" + count] = bill.LateCharge;
                count++;
            }
            ViewData["Maintenence"] = Convert.ToDecimal(per_square_foot);
            
            return View(billsofuser);
        }

        private void CalculateLateFees(int resid)
        {
            var billsofuser = _db.bills_payment.Where(b=>b.ResidentId == resid && b.PaidAmount == 0 && b.Bill.DueDate < DateTime.Now).Include(b => b.Bill).ToList();
            var late_charge = 0;
            foreach (var bill in billsofuser)
            {
                var DateDiff = DateTime.Now - bill.Bill.DueDate;
                if (DateDiff.Days.Equals(1))
                {
                    late_charge = Convert.ToInt32(bill.Amount * 0.05);
                    bill.LateCharge = late_charge;
                    bill.Amount = bill.Amount + late_charge;
                }
                else if (DateDiff.Days > 1)
                {
                    bill.Amount = Convert.ToInt32(bill.Amount + bill.LateCharge*DateDiff.Days);
                }
                _db.bills_payment.Update(bill);
                _db.SaveChanges();
            }
        }

        // GET: BillsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: BillsController/Create
        public ActionResult Create(int id)
        {
            BillsPayment bill = _db.bills_payment.FirstOrDefault(b => b.BillPaymentId == id);
            PaymentModel options = new PaymentModel
            {
                Id = bill.BillPaymentId,
                Name = "Jeshan Patel",
                Email = "jeshanpatel123@gmail.com",
                Contact = "7069170879",
                Amount = bill.Amount*100,
                Amount_Rs = bill.Amount,
                Currency = "INR",
                Payment_Capture = 1,
                Notes = "Paid!!",
                OrderId = "",
                Key = "",
                ImageURL = "/images/Logo.png"
            };

            var orderid = Payment(options);
            options.OrderId = orderid;
            options.Key = _key;
            return View(options);
        }


        private string Payment(PaymentModel payment)
        {
            try
            {

                RazorpayClient client = new RazorpayClient(_key, _secret);
                Dictionary<string, object> options = new Dictionary<string, object>();
                options.Add("amount", Convert.ToInt32(payment.Amount) * 100);
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
        public async Task<ActionResult> PaymentEdit(int bill_id, string payment_id, int amount)
        {
            string URL = "http://localhost:5205/EmailService";
            try
            {
                var bill = _db.bills_payment.Include(b=>b.Resident).FirstOrDefault(b => b.BillPaymentId == bill_id);
                bill.PaymentId = payment_id;
                bill.PaidAmount = amount;
                bill.Status = true;
                bill.PaidOn = DateTime.Parse(DateTime.Now.ToLongDateString());
                _db.bills_payment.Update(bill);
                var bill2 = _db.Bill.FirstOrDefault(b => b.BillId.Equals(_db.bills_payment.FirstOrDefault(bp => bp.BillPaymentId == bill_id).BillId));
                _db.bills.FirstOrDefault(b => b.BillId.Equals(_db.bills_payment.FirstOrDefault(bp => bp.BillPaymentId == bill_id).BillId)).Paid += 1;
                _db.bills.FirstOrDefault(b => b.BillId.Equals(_db.bills_payment.FirstOrDefault(bp => bp.BillPaymentId == bill_id).BillId)).Unpaid -= 1;
                _db.SaveChanges();
                TempData["success"] = "Bill Paid Successfully!!";

                string Data = "Bill : " + bill2.Title 
                    + "\nResident : " + bill.Resident.FirstName + " " + bill.Resident.LastName 
                    + "\nAmount : " + bill.Amount 
                    + "\nPaymentId : #" + bill.PaymentId 
                    + "\nPayment Date : " + bill.PaidOn;

                EmailData email = new EmailData();
                email.To = bill.Resident.Email;
                email.Subject = "Bill Payment Reciept";
                email.Body = Data;

                using(HttpClient http = new HttpClient())
                {
                    var newData = JsonConvert.SerializeObject(email);
                    HttpContent content = new StringContent(newData, Encoding.UTF8, "application/json");
                    using (var res = await http.PostAsync(URL, content))
                    {
                        var apires = await res.Content.ReadAsStringAsync();
                    }
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: BillsController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: BillsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: BillsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: BillsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
    public class PaymentModel
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }
        public int Amount { get; set; }
        public int Amount_Rs { get; set; }
        public string Currency { get; set; }
        public int Payment_Capture { get; set; }
        public string Notes { get; set; }
        public string OrderId { get; set; }
        public string ImageURL { get; set; }
    }
}
