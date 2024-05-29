using LodgeLink.Data;
using LodgeLink.Models;
using Microsoft.AspNetCore.Mvc;

namespace LodgeLink.Areas.Events.Controllers
{
    public class EventManageController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventManageController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetEvents()
        {
            var events = _context.events.ToList();
            return new JsonResult(events);
            //return new JsonResult { Data = events, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }
        [HttpPost]
        public JsonResult SaveEvent(Event e)
        {
            var status = false;

            if (e.EventId > 0)
            {
                //Update the event
                var v = _context.events.Where(a => a.EventId == e.EventId).FirstOrDefault();
                if (v != null)
                {
                    v.Subject = e.Subject;
                    v.Start = e.Start;
                    v.End = e.End;
                    v.Description = e.Description;
                    v.IsFullDay = e.IsFullDay;
                    v.ThemeColor = e.ThemeColor;
                    v.Fees = e.Fees;
                }
            }
            else
            {
                _context.events.Add(e);
            }
            _context.SaveChanges();
            status = true;
            return new JsonResult(new { status = status });
        }
        [HttpPost]
        public JsonResult DeleteEvent(int eventID)
        {
            var status = false;

            var v = _context.events.Where(a => a.EventId == eventID).FirstOrDefault();
            if (v != null)
            {
                _context.events.Remove(v);
                _context.SaveChanges();
                status = true;
            }
            return new JsonResult(new { status = status });
        }
    }
}
