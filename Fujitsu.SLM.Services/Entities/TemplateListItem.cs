using System;

namespace Fujitsu.SLM.Services.Entities
{
    public class TemplateListItem
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public string TemplateType { get; set; }
        public string InsertedBy { get; set; }
        public DateTime InsertedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}