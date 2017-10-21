using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class BulkResolverLevelZeroViewModel : BaseBulkResolverViewModel
    {

        [Display(Name = "Service Desk")]
        public int ServiceDeskId { get; set; }

        [Display(Name = "Services")]
        [UIHint("WindowMultiLine")]
        public string ServiceDeliveryUnitNotes { get; set; }
    }
}