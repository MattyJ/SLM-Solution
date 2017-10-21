using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fujitsu.SLM.Model
{
    public class ContextHelpRefData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string Key { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        public string HelpText { get; set; }

        public int? AssetId { get; set; }
        public virtual Asset Asset { get; set; }
    }
}
