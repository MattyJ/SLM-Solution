using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class EditServiceComponentLevel2ViewModelValidator : AbstractValidator<EditServiceComponentLevel2ViewModel>
    {
        public EditServiceComponentLevel2ViewModelValidator()
        {
            RuleFor(x => x.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationTypeId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull()
                .WithMessage(WebResources.ServiceComponentServiceDeliveryOrganisationTypeIdMustBeSupplied)
                .GreaterThan(0)
                .When(x => !string.IsNullOrEmpty(x.ResolverServiceDeliveryOrganisation.ServiceDeliveryOrganisationNotes) || (x.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId != null && x.ResolverServiceDeliveryUnit.ServiceDeliveryUnitTypeId > 0) ||
                        (x.ResolverGroup.ResolverGroupTypeId != null && x.ResolverGroup.ResolverGroupTypeId > 0));

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