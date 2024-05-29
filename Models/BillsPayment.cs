using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LodgeLink.Models
{
    public class BillsPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BillPaymentId { get; set; }
        [Required]
        public int BillId { get; set; }
        [ForeignKey("BillId")]
        [ValidateNever]
        public Bill Bill { get; set; }
        [Required]
        public int ResidentId { get; set; }
        [ForeignKey("ResidentId")]
        [ValidateNever]
        public Resident Resident { get; set; }
        [Required]
        public int Amount { get; set; }
        public string? PaymentId { get; set; }
        public int? PaidAmount { get; set; }
        public int? LateCharge { get; set; }
        [Required]
        public Boolean Status { get; set; }
        public DateTime? PaidOn { get; set; }
    }
}
