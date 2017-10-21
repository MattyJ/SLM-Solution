using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class BulkServiceFunctionViewModel : BaseServiceFunctionViewModel
    {
        [Display(Name = "Service Domain")]
        public int? ServiceDomainId { get; set; }
    }
}