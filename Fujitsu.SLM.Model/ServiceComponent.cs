using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class ServiceComponent : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        public int ComponentLevel { get; set; }

        public virtual ICollection<ServiceComponent> ChildServiceComponents { get; set; }

        public int? ParentServiceComponentId { get; set; }
        public virtual ServiceComponent ParentServiceComponent { get; set; }

        [Required]
        [StringLength(100)]
        public string ComponentName { get; set; }

        public int DiagramOrder { get; set; }

        [StringLength(1500)]
        public string ServiceActivities { get; set; }

        public int ServiceFunctionId { get; set; }
        public virtual ServiceFunction ServiceFunction { get; set; }

        // Foreign Key
        public virtual Resolver Resolver { get; set; }

    }
}