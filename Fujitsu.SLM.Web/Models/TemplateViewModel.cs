namespace Fujitsu.SLM.Web.Models
{
    public class TemplateViewModel : InsertedUpdatedViewModel
    {
        public int Id { get; set; }

        public string Filename { get; set; }

        public string TemplateType { get; set; }
    }
}