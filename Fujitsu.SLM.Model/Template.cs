using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class Template : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Filename { get; set; }

        [Required]
        public int TemplateType { get; set; }

        [Required]
        public byte[] TemplateData { get; set; }

        public virtual ICollection<TemplateDomain> TemplateDomains { get; set; }

        public virtual ICollection<TemplateRow> TemplateRows { get; set; }
    }
}