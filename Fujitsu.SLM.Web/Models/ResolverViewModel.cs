using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class ResolverViewModel
    {
        public int Id { get; set; }

        public int ServiceDeskId { get; set; }

        [Display(Name = "Service Desk")]
        public string ServiceDeskName { get; set; }

        [Display(Name = "Service Domain")]
        public string ServiceDomainName { get; set; }

        [Display(Name = "Service Function")]
        public string ServiceFunctionName { get; set; } // Name or alternative

        public int ServiceComponentId { get; set; }

        [Display(Name = "Service Component")]
        public string ServiceComponentName { get; set; }

        [Display(Name = "Service Delivery Organisation")]
        public string ServiceDeliveryOrganisationTypeName { get; set; }

        [Display(Name = "Service Delivery Unit")]
        public string ServiceDeliveryUnitTypeName { get; set; }

        [Display(Name = "Resolver Group")]
        public string ResolverGroupName { get; set; }
    }
}