using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditServiceComponentChildViewModelValidator))]
    public class EditServiceComponentChildViewModel
    {
        [Display(Name = "Component Name")]
        public string ComponentName { get; set; }

        [Display(Name = "Diagram Order")]
        [UIHint("SmallInteger")]
        public int? DiagramOrder { get; set; }
    }
}