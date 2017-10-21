using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Model
{
    public class Diagram : BaseEntity
    {
        public int Id { get; set; }

        public int Level { get; set; }

        [Required]
        [StringLength(250)]
        public string Filename { get; set; }

        [Required]
        [StringLength(100)]
        public string MimeType { get; set; }

        [Required]
        public byte[] DiagramData { get; set; }

        [StringLength(250)]
        public string DiagramNotes { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
