using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class TemplateComponent : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int ComponentLevel { get; set; }

        public virtual ICollection<TemplateComponent> ChildTemplateComponents { get; set; }

        public virtual ICollection<TemplateResolver> TemplateResolvers { get; set; }

        public int? ParentTemplateComponentId { get; set; }
        public virtual TemplateComponent ParentTemplateComponent { get; set; }

        [Required]
        [StringLength(100)]
        public string ComponentName { get; set; }

        [StringLength(1500)]
        public string ServiceActivities { get; set; }

        // Foreign key
        public int TemplateFunctionId { get; set; }
        public virtual TemplateFunction TemplateFunction { get; set; }
    }
}