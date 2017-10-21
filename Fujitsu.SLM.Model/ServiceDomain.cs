using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class ServiceDomain : BaseEntity
    {
        public int Id { get; set; }

        public virtual ICollection<ServiceFunction> ServiceFunctions { get; set; }

        // Foreign Key
        public int DomainTypeId { get; set; }
        public virtual DomainTypeRefData DomainType { get; set; }

        [StringLength(100)]
        public string AlternativeName { get; set; }

        public int DiagramOrder { get; set; }

        // Foreign Key
        public int ServiceDeskId { get; set; }
        public virtual ServiceDesk ServiceDesk { get; set; }
    }
}
