using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.SLM.Model
{
    public class InputTypeRefData
    {
        public int Id { get; set; }

        [Required]
        public int InputTypeNumber { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string InputTypeName { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public bool Default { get; set; }
    }
}
