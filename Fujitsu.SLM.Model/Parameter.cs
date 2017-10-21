using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fujitsu.SLM.Enumerations;

namespace Fujitsu.SLM.Model
{
    public class Parameter : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Index(IsUnique = true)]
        public string ParameterName { get; set; }

        [Required]
        [StringLength(200)]
        public string ParameterValue { get; set; }

        [Required]
        public ParameterType Type { get; set; }
    }
}
