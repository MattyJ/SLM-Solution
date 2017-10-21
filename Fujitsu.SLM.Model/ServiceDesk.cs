using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class ServiceDesk : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DeskName { get; set; }

        public virtual ICollection<DeskInputType> DeskInputTypes { get; set; }

        public virtual ICollection<ServiceDomain> ServiceDomains { get; set; }

        public virtual ICollection<Resolver> Resolvers { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
