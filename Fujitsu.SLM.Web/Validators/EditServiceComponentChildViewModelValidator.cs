using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class EditServiceComponentChildViewModelValidator : AbstractValidator<EditServiceComponentChildViewModel>
    {
        public EditServiceComponentChildViewModelValidator()
        {
            RuleFor(x => x.ComponentName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.ServiceComponentName);

            RuleFor(x => x.DiagramOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .InclusiveBetween(1, Validation.MaxValue.DiagramOrder);
        }
    }
}
