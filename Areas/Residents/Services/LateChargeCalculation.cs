using LodgeLink.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LodgeLink.Areas.Residents.Services
{
    public class LateChargeCalculation : BackgroundService
    {
        private readonly ApplicationDbContext _db;
        private ILogger<LateChargeCalculation> _logger;
        private readonly int resid;
        public LateChargeCalculation(ApplicationDbContext db, ILogger<LateChargeCalculation> logger)
        {
            _db = db;
            _logger = logger;

        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var billsofuser = _db.bills_payment.Include(b => b.Resident).Include(b => b.Bill).ToList();
            var late_charge = 0;
            foreach (var bill in billsofuser)
            {
                bill.LateCharge = 0;
                if (bill.PaidAmount == 0)
                {
                    var DateDiff = DateTime.Now - bill.Bill.DueDate;
                    if (DateDiff.Days.Equals(1))
                    {
                        late_charge = (int)(bill.Amount * 0.05);
                        bill.LateCharge = late_charge;
                        bill.Amount = bill.Amount + late_charge;
                    }
                    else if (DateDiff.Days > 1)
                    {
                        bill.Amount = Convert.ToInt32(bill.Amount + bill.LateCharge);
                    }
                }
                _db.bills_payment.Update(bill);
                _db.SaveChanges();
            }
            await Task.Delay(5000);
        }
    }
}
