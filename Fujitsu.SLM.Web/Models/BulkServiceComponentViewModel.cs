using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class BulkServiceComponentViewModel : BaseServiceComponentViewModel
    {
        [Display(Name = "Service Component (Level 1)")]
        public int? ServiceComponentLevel1Id { get; set; }

        [Display(Name = "Service Function")]
        public int? ServiceFunctionId { get; set; }
    }
}