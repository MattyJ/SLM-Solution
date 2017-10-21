using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class ServiceActivityViewModelValidator : AbstractValidator<ServiceActivityViewModel>
    {
        public ServiceActivityViewModelValidator()
        {
            RuleFor(x => x.ServiceActivities)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.ServiceActivities);
        }
    }
}