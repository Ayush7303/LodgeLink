using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class Property
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PropertyId { get; set; }
        [Required]
        public string PeopertyNumber { get; set; }
        public string PropertyType { get; set; }
        public decimal PropertySize { get; set; }
        public string PropertyStatus { get; set; }
        //public int PropertyOwnerId { get; set; }
        //[ForeignKey("ResidentId")]
        //public Resident Resident { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PropertyValue { get; set; }
        public string PropertyFeatures { get; set; }
        public int BuildingId { get; set; }
        [ForeignKey("BuildingId")]
        [ValidateNever]
        public Building Building { get; set; }
        public string Description { get; set; }
    }
}
