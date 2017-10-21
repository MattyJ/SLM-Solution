using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class ServiceComponentViewModel : BaseServiceComponentViewModel
    {
        [Display(Name = "Service Function")]
        public int ServiceFunctionId { get; set; }
        public int? ParentServiceComponentId { get; set; }
    }
}