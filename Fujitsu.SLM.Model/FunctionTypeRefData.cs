using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Model
{
    public class FunctionTypeRefData
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Index(IsUnique = true)]
        public string FunctionName { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public bool Visible { get; set; }
    }
}
