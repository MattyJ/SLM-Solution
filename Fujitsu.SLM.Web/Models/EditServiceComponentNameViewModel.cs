using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(EditServiceComponentNameViewModelValidator))]
    public class EditServiceComponentNameViewModel
    {
        [Display(Name = "Component Name")]
        public string ComponentName { get; set; }
    }
}