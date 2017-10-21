using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Extensions;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class EditServiceComponentNameLevelViewModelValidator : AbstractValidator<EditServiceComponentNameLevelViewModel>
    {
        public EditServiceComponentNameLevelViewModelValidator()
        {
            RuleFor(x => x.ComponentName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.ServiceComponentName);

            RuleFor(x => x.ComponentLevel)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(WebResources.ServiceComponentLevelPleaseSelect)
                .Contains(ServiceComponentLevelNames.Values)
                .WithMessage(WebResources.ServiceComponentLevelInvalid);

            RuleFor(x => x.DiagramOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .InclusiveBetween(1, Validation.MaxValue.DiagramOrder);
        }
    }
}