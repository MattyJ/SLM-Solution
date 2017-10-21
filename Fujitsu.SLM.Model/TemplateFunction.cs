using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class TemplateFunction : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FunctionName { get; set; }

        public virtual ICollection<TemplateComponent> TemplateComponents { get; set; }

        // Foreign key
        public int TemplateDomainId { get; set; }
        public virtual TemplateDomain TemplateDomain { get; set; }
    }
}