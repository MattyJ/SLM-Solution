using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class ServiceFunction : BaseEntity
    {
        public int Id { get; set; }

        public virtual ICollection<ServiceComponent> ServiceComponents { get; set; }

        // Foreign Key
        public int FunctionTypeId { get; set; }
        public virtual FunctionTypeRefData FunctionType { get; set; }

        [StringLength(100)]
        public string AlternativeName { get; set; }

        public int DiagramOrder { get; set; }

        // Foreign key
        public int ServiceDomainId { get; set; }
        public virtual ServiceDomain ServiceDomain { get; set; }
    }
}