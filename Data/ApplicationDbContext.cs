using LodgeLink.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace LodgeLink.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 
        
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Property>().ToTable(tb => tb.HasTrigger("insertincred"));
            builder.Entity<Bill>().ToTable(tb => tb.HasTrigger("After_Bill_Create"));
        }
        public DbSet<Building> buildings { get; set; }
        public DbSet<Property> properties { get; set; }
        public DbSet<Credentials> credentials { get; set; }
        public DbSet<Resident> residents { get; set; }
        public DbSet<Bill> bills { get; set; }
        public DbSet<BillsPayment> bills_payment { get; set; }
        public DbSet<Announcement> announcements { get; set; }
        public DbSet<Role> roles { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Visitor> visitors { get; set; }
        public DbSet<ArchitecturalRequest> architecturalRequests { get; set; }
        public DbSet<Facility> facilities { get; set; }
        public DbSet<BookingRequest> bookingRequests { get; set; }
        public DbSet<Event> events { get; set; }
        public DbSet<EventParticipant> eventParticipants { get; set; }
        public DbSet<MaintenanceRequest> maintenanceRequests { get; set; }
        public DbSet<LodgeLink.Models.Bill> Bill { get; set; } = default!;
        public DbSet<LodgeLink.Models.Role> Role { get; set; } = default!;
        public DbSet<LodgeLink.Models.User> User { get; set; } = default!;
        public DbSet<LodgeLink.Models.Facility> Facility { get; set; } = default!;


    }
}
