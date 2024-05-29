using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace LodgeLink.Models
{
    public class Requests
    {
        public string Category { get; set; }
        public string Description { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Notes { get; set; }

        [ValidateNever]
        public string? Attachments { get; set; }
    }
}
