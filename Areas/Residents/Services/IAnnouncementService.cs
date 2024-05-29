using LodgeLink.Models;

namespace LodgeLink.Areas.Residents.Services
{
    public interface IAnnouncementService
    {
        List<Announcement> GetAnnouncements();
    }
}
