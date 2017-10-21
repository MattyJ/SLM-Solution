using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class Resolver : BaseEntity
    {
        public int Id { get; set; }

        // Foreign Key
        public int ServiceDeskId { get; set; }
        public virtual ServiceDesk ServiceDesk { get; set; }

        // Service Component
        public virtual ServiceComponent ServiceComponent { get; set; }

        // Service Delivery Organisation
        public int ServiceDeliveryOrganisationTypeId { get; set; }
        public virtual ServiceDeliveryOrganisationTypeRefData ServiceDeliveryOrganisationType { get; set; }

        [StringLength(250)]
        public string ServiceDeliveryOrganisationNotes { get; set; }

        // Service Delivery Unit
        public virtual ServiceDeliveryUnitTypeRefData ServiceDeliveryUnitType { get; set; }

        [StringLength(1500)]
        public string ServiceDeliveryUnitNotes { get; set; }

        // Resolver Group
        public virtual ResolverGroupTypeRefData ResolverGroupType { get; set; }

        // Operational Processes
        public virtual ICollection<OperationalProcessType> OperationalProcessTypes { get; set; }
    }
}