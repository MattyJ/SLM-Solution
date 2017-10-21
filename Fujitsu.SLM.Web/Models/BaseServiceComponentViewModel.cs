using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public abstract class BaseServiceComponentViewModel : InsertedUpdatedViewModel
    {
        public bool CanDelete { get; set; }

        [Display(Name = "Service Desk")]
        public string ServiceDeskName { get; set; }

        [Display(Name = "Service Domain")]
        public string ServiceDomainName { get; set; }

        [Display(Name = "Service Function")]
        public string ServiceFunctionName { get; set; }

        public int Id { get; set; }

        public int ComponentLevel { get; set; }

        [Display(Name = "Service Component")]
        public string ComponentName { get; set; }

        [Display(Name = "Diagram Order")]
        [UIHint("SmallInteger")]
        public int? DiagramOrder { get; set; }

        [Display(Name = "Service Activities")]
        public string ServiceActivities { get; set; }
    }
}