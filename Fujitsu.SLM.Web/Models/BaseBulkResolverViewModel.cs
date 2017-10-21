using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class BaseBulkResolverViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Service Delivery Organisation")]
        public int ServiceDeliveryOrganisationTypeId { get; set; }

        [Display(Name = "Service Delivery Unit")]
        public int ServiceDeliveryUnitTypeId { get; set; }

        [Display(Name = "Resolver Group")]
        public int ResolverGroupTypeId { get; set; }
    }
}