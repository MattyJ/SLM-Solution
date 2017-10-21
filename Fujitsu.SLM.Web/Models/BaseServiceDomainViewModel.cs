using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public abstract class BaseServiceDomainViewModel : UpdatedViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Service Desk")]
        public string ServiceDeskName { get; set; }

        [Display(Name = "Service Domain")]
        public int DomainTypeId { get; set; }

        [Display(Name = "Service Domain")]
        public string DomainName { get; set; }

        [Display(Name = "Alternative Name")]
        public string AlternativeName { get; set; }

        [Display(Name = "Diagram Order")]
        [UIHint("SmallInteger")]
        public int? DiagramOrder { get; set; }
    }
}