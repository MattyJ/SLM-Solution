using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class MoveResolverLevelZeroViewModelValidator : AbstractValidator<MoveResolverLevelZeroViewModel>
    {
        public MoveResolverLevelZeroViewModelValidator()
        {
            RuleFor(x => x.DestinationDeskId)
                   .Cascade(CascadeMode.StopOnFirstFailure)
                   .NotNull()
                   .WithMessage(WebResources.ServiceDeskPleaseSelect)
                   .GreaterThan(0)
                   .WithMessage(WebResources.ServiceDeskPleaseSelect);
        }
    }
}
