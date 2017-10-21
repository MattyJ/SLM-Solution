using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.SLM.Model
{
    public class Customer : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string CustomerName { get; set; }

        [StringLength(250)]
        public string CustomerNotes { get; set; }

        [Required]
        [StringLength(150)]
        public string AssignedArchitect { get; set; }

        public virtual ICollection<Contributor> Contributors { get; set; }

        [Required]
        public bool Baseline { get; set; }

        [Required]
        public bool Active { get; set; }

        public virtual ICollection<ServiceDesk> ServiceDesks { get; set; }

        public virtual ICollection<Audit> Audits { get; set; }

    }
}
