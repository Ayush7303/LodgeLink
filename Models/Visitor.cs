using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class Visitor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VisitorId { get; set; }
        [Required]
        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        public Property? Property { get; set; }
        [Required]
        public string VisitorName { get; set; }
        [ValidateNever]
        public string? ImageURL { get; set; }
        public DateTime? VisitDate { get; set; }
        public string Purpose { get; set; }
        public string ContactNumber { get; set; }
        public string VehicleDetails { get; set; }
        public int? ApprovedBy { get; set; }
        [ForeignKey("UserId")]
        public User? User { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
    }
}
