using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class TemplateDomainViewModel
    {
        public int? ServiceDeskId { get; set; }
        public bool Selected { get; set; }
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string DomainName { get; set; }
        public int TemplateId { get; set; }
    }
}