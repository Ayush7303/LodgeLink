using LodgeLink.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace LodgeLink.Areas.Admin.Controllers
{
    
    public class EventParticipantAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventParticipantAdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var Request2 = _context.eventParticipants.Include(x => x.Resident).Include(x => x.Event).ToList();

            var Request1 = _context.eventParticipants.GroupBy(ep => ep.EventId)
    .Select(group => group.First())
    .ToList();

            dynamic myModel = new ExpandoObject();
            myModel.Event = Request1;
            myModel.EventParticipants = Request2;
            return View(myModel);
        }
    }
}
