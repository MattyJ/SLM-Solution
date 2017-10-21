using System.Collections.Generic;

namespace Fujitsu.SLM.Model
{
    public class Asset
    {
        public int Id { get; set; }

        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public string OriginalFileName { get; set; }

        public string FullPath { get; set; }

        public string MimeType { get; set; }

        public virtual ICollection<ContextHelpRefData> ContextHelpRefDatas { get; set; }
    }
}