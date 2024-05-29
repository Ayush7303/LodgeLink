using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LodgeLink.Models
{
    public class Credentials
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CredentialId { get; set; }
        public int BuildingId { get; set; }
        [ForeignKey("BuildingId")]
        [ValidateNever]
        public Building Building { get; set; }
        public int PropertyId { get; set; }
        [ForeignKey("PropertyId")]
        [ValidateNever]
        public Property Property { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
