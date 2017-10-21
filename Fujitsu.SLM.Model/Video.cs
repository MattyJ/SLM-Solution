using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Model
{
    public class Asset : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Filename { get; set; }

        [Required]
        public byte[] AssetData { get; set; }

        // Foreign Key
        public int MimeTypeId { get; set; }
        public virtual MimeTypeRefData MimeType { get; set; }
    }

    public class MimeTypeRefData : BaseEntity
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string MimeType { get; set; }

        [Required]
        [StringLength(100)]
        public string AssetType { get; set; }

        public bool IsObsolete { get; set; }
    }
}
