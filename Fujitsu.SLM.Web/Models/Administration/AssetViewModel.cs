using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class AssetViewModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        [Display(Name = "File Extension")]
        public string FileExtension { get; set; }

        [Display(Name = "FileName")]
        public string OriginalFileName { get; set; }

        public string FullPath { get; set; }

        [Display(Name = "Mime Type")]
        public string MimeType { get; set; }

    }
}