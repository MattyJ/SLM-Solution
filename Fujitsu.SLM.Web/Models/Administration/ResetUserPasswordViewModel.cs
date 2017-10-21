using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ResetUserPasswordViewModelValidator))]
    public class ResetUserPasswordViewModel
    {
        public string UserId { get; set; }
    }
}