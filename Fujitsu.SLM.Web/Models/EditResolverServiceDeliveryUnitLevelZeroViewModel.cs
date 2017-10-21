using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class EditResolverServiceDeliveryUnitLevelZeroViewModel
    {
        [Display(Name = "SDU Type")]
        public int? ServiceDeliveryUnitTypeId { get; set; }

        [Display(Name = "Services")]
        public string ServiceDeliveryUnitNotes { get; set; }
    }
}
