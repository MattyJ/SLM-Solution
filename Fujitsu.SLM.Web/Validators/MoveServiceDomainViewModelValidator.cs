using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class MoveServiceDomainViewModelValidator : AbstractValidator<MoveServiceDomainViewModel>
    {
        public MoveServiceDomainViewModelValidator()
        {
            RuleFor(x => x.ServiceDeskId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage(WebResources.ServiceDeskPleaseSelect)
                .GreaterThan(0);

            RuleFor(x => x.ServiceDomainId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0);
        }
    }
}