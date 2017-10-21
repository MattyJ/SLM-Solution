using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class CustomerPack : BaseEntity
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
        public byte[] PackData { get; set; }

        [StringLength(250)]
        public string PackNotes { get; set; }

        // Foreign key
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; }
    }
}