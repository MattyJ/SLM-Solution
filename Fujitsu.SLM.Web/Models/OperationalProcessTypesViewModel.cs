using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class OperationalProcessTypesViewModel
    {
        [Display(Name = "Operational Processes")]
        public int[] OperationalProcessTypes { get; set; }
    }
}