using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class TemplateResolver : BaseEntity
    {
        public int Id { get; set; }

        // Service Delivery Organisation
        [Required]
        [StringLength(100)]
        public string ServiceDeliveryOrganisationName { get; set; }

        // Service Delivery Unit
        [StringLength(100)]
        public string ServiceDeliveryUnitName { get; set; }

        // Resolver Group
        [StringLength(100)]
        public string ResolverGroupName { get; set; }

        // Foreign key
        public int TemplateComponentId { get; set; }
        public virtual TemplateComponent TemplateComponent { get; set; }
    }
}