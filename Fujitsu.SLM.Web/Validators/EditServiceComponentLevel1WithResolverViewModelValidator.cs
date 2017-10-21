using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class EditServiceComponentLevel1WithResolverViewModelValidator : AbstractValidator<EditServiceComponentLevel1WithResolverViewModel>
    {
        public EditServiceComponentLevel1WithResolverViewModelValidator()
        {
            RuleFor(x => x.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage(WebResources.ServiceComponentServiceDeliveryOrganisationTypeIdMustBeSupplied)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceComponentServiceDeliveryOrganisationTypeIdMustBeSupplied);

            RuleFor(x => x.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage(WebResources.ServiceComponentServiceDeliveryUnitTypeIdMustBeSupplied)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceComponentServiceDeliveryUnitTypeIdMustBeSupplied)
                .When(x => !string.IsNullOrEmpty(x.ResolverServiceDeliveryUnit.ServiceDeliveryUnitNotes));
        }
    }
}
