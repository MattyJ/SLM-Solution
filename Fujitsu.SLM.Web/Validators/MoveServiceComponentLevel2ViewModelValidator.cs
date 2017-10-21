using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class MoveServiceComponentLevel2ViewModelValidator : AbstractValidator<MoveServiceComponentLevel2ViewModel>
    {
        public MoveServiceComponentLevel2ViewModelValidator()
        {
            RuleFor(x => x.DestinationServiceComponentId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage(WebResources.ServiceComponentPleaseSelect)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceComponentPleaseSelect);

            RuleFor(x => x.ServiceComponentId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceComponentPleaseSelect);
        }
    }
}