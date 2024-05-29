using LodgeLink.Data;
using LodgeLink.Models;

namespace LodgeLink.Areas.Residents.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly ApplicationDbContext _db;

        public AnnouncementService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<Announcement> GetAnnouncements()
        {
            var announcements = _db.announcements.ToList();
            return announcements;
        }
    }
}
