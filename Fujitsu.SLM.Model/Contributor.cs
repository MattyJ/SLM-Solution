using System;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class Contributor : BaseEntity
    {
        public int Id { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(150)]
        public string EmailAddress { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
