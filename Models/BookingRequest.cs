using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class BookingRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        [Required]
        public int ResidentId { get; set; }
        [ForeignKey("ResidentId")]
        public Resident? Resident { get; set; }
        [Required]
        public int FacilityId { get; set; }
        [ForeignKey("FacilityId")]
        public Facility? Facility { get; set; }
        public DateTime? RequestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public double PaidAmount { get; set; }
        public string PaymentId { get; set; }
        public string? Notes { get; set; }
        public string? AdminComments { get; set; }
        public Boolean? NotificationSent { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string? CancellationReason { get; set;}
        public DateTime? LastUpdated { get; set; }
    }
}
