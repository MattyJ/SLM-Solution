using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.SLM.Model
{
    public class DomainTypeRefData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string DomainName { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public bool Visible { get; set; }
    }
}
