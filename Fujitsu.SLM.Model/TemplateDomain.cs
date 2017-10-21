using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class TemplateDomain : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DomainName { get; set; }

        public virtual ICollection<TemplateFunction> TemplateFunctions { get; set; }

        // Foreign Key
        public int TemplateId { get; set; }
        public virtual Template Template { get; set; }
    }
}