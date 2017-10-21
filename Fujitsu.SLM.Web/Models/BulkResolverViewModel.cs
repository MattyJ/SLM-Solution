using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class BulkResolverViewModel : BaseBulkResolverViewModel
    {
        [Display(Name = "Service Component")]
        public int? ServiceComponentId { get; set; }
    }
}