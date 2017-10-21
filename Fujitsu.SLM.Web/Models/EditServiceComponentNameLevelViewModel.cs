using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditServiceComponentNameLevelViewModelValidator))]
    public class EditServiceComponentNameLevelViewModel
    {
        [Display(Name = "Component Name")]
        public string ComponentName { get; set; }

        [Display(Name = "Component Level")]
        public string ComponentLevel { get; set; }

        [Display(Name = "Diagram Order")]
        [UIHint("SmallInteger")]
        public int? DiagramOrder { get; set; }
    }
}