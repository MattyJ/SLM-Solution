using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class EditServiceComponentNameViewModelValidator : AbstractValidator<EditServiceComponentNameViewModel>
    {
        public EditServiceComponentNameViewModelValidator()
        {
            RuleFor(x => x.ComponentName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.ServiceComponentName);
        }
    }
}
