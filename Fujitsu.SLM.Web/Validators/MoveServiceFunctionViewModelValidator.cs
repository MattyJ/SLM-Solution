using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class MoveServiceFunctionViewModelValidator : AbstractValidator<MoveServiceFunctionViewModel>
    {
        public MoveServiceFunctionViewModelValidator()
        {
            RuleFor(x => x.ServiceDomainId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage(WebResources.ServiceDomainPleaseSelect)
                .GreaterThan(0);

            RuleFor(x => x.ServiceFunctionId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0);
        }
    }
}