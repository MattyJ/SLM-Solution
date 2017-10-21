using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class ServiceDeliveryOrganisationTypeRefDataViewModelValidator : AbstractValidator<ServiceDeliveryOrganisationTypeRefDataViewModel>
    {
        public ServiceDeliveryOrganisationTypeRefDataViewModelValidator()
        {
            RuleFor(x => x.ServiceDeliveryOrganisationTypeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.ServiceDeliveryOrganisationName);

            RuleFor(x => x.SortOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.SortOrder);
        }
    }
}