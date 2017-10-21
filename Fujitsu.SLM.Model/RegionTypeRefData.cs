using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.SLM.Model
{
    public class RegionTypeRefData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string RegionName { get; set; }

        [Required]
        public int SortOrder { get; set; }
    }
}