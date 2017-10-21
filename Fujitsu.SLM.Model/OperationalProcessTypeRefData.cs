using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.SLM.Model
{
    public class OperationalProcessTypeRefData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string OperationalProcessTypeName { get; set; }

        [Required]
        public int SortOrder { get; set; }

        [Required]
        public bool Visible { get; set; }

        public bool Standard { get; set; }
    }
}