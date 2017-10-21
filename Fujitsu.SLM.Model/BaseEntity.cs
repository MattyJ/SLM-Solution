using System;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public abstract class BaseEntity
    {
        [Required]
        public DateTime InsertedDate { get; set; }

        [Required]
        [StringLength(150)]
        public string InsertedBy { get; set; }

        [Required]
        public DateTime UpdatedDate { get; set; }

        [Required]
        [StringLength(150)]
        public string UpdatedBy { get; set; }
    }
}