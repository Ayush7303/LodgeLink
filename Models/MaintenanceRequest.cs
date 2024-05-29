using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class MaintenanceRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }
        [Required]
        public int ResidentId { get; set; }
        [ForeignKey("ResidentId")]
        public Resident? Resident { get; set; }
        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public Property? Property { get; set; }
        public DateTime? RequestDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? Priority { get; set; }
        public string? Category { get; set; }
        public string? Notes { get; set; }
        public string? Attachments { get; set; }
    }
}
