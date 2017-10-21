using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class BulkServiceDomainViewModel : BaseServiceDomainViewModel
    {
        [Display(Name = "Service Desk")]
        public int? ServiceDeskId { get; set; }
    }
}