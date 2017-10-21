using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public abstract class EditServiceComponentViewModel : LevelViewModel
    {
        public int Id { get; set; }
        public string EditUrl { get; set; }
        public string ParentName { get; set; }
    }
}