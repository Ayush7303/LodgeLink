using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class EventParticipant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParticipantId { get; set; }
        [Required]
        public int EventId { get; set; }
        [ForeignKey("EventId")]
        public Event? Event { get; set; }
        [Required]
        public int ResidentId { get; set; }
        [ForeignKey("ResidentId")]
        public Resident? Resident { get; set; }
        [Required]
        public DateTime RegistrationDate { get; set; }
        [Required]
        public string? Status { get; set; }
        public int Members { get; set; }
        public int PaidAmount { get; set; }
        public string? PaymentId { get; set; }
        public string? Feedback { get; set; }
        public int Rating { get; set; }
    }
}
