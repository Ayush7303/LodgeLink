using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class Resident
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ResidentId { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        public string? ContactNumber { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        [ValidateNever]
        public Property Property { get; set; }
        public int CredentialId { get; set; }
        [ForeignKey("CredentialId")]
        [ValidateNever]
        public Credentials Credentials { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfMoveIn { get; set; }
        public string? EmergencyContactNumber { get; set; }
        public string? ProfileImage { get; set; }
        public Boolean? IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set;}
        public string? Status { get; set;}

    }
}
