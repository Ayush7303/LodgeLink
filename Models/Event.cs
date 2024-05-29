using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LodgeLink.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }
        public string? Subject { get; set; }
        public string? Description { get; set; }     
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string? ThemeColor { get; set; }
        public Boolean? IsFullDay { get; set; }
        public double? Fees { get; set; }
    }
}
