using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class TemplateRowViewModel
    {
        [Display(Name = "Service Domain")]
        public string ServiceDomain { get; set; }

        [Display(Name = "Service Function")]
        public string ServiceFunction { get; set; }

        [Display(Name = "Service Component Level 1")]
        public string ServiceComponentLevel1 { get; set; }

        [Display(Name = "Service Component Level 2")]
        public string ServiceComponentLevel2 { get; set; }

        [Display(Name = "Service Activities")]
        public string ServiceActivities { get; set; }

        [Display(Name = "Service Delivery Organisation")]
        public string ServiceDeliveryOrganisation { get; set; }

        [Display(Name = "Service Delivery Unit")]
        public string ServiceDeliveryUnit { get; set; }

        [Display(Name = "Resolver Group")]
        public string ResolverGroup { get; set; }
    }
}