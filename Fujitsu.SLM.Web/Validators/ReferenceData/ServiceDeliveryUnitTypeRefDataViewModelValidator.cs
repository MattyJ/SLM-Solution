using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class ServiceDeliveryUnitTypeRefDataViewModelValidator : AbstractValidator<ServiceDeliveryUnitTypeRefDataViewModel>
    {
        public ServiceDeliveryUnitTypeRefDataViewModelValidator()
        {
            RuleFor(x => x.ServiceDeliveryUnitTypeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.ServiceDeliveryUnitName);

            RuleFor(x => x.SortOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.SortOrder);
        }
    }
}