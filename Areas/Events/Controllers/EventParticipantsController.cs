using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LodgeLink.Data;
using LodgeLink.Models;
using System.Dynamic;

namespace LodgeLink.Areas.Events.Controllers
{
    public class EventParticipantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventParticipantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events/EventParticipants
        public async Task<IActionResult> Index()
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
