using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class MoveServiceComponentLevel1ViewModelValidator : AbstractValidator<MoveServiceComponentLevel1ViewModel>
    {
        public MoveServiceComponentLevel1ViewModelValidator()
        {
            RuleFor(x => x.DestinationServiceFunctionId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage(WebResources.ServiceComponentPleaseSelect)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceFunctionPleaseSelect);

            RuleFor(x => x.ServiceComponentId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceComponentPleaseSelect);
        }
    }
}