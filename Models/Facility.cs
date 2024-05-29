using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class Facility
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FacilityId { get; set; }
        [Required]
        public string FacilityName { get; set; }
        public string FacilityType { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
        public string Availability { get; set; }
        public string Rules { get; set; }
        public Decimal BookingFee { get; set; }
        [ValidateNever]
        public string ImageURL { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set;}
    }
}
