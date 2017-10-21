using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class Audit : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public double Version { get; set; }

        [Required]
        [StringLength(100)]
        public string ReasonForIssue { get; set; }

        [StringLength(250)]
        public string Notes { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
