using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class BaseServiceDeliveryOrganisationTypeViewModelValidator : AbstractValidator<BaseServiceDeliveryOrganisationTypeViewModel>
    {
        public BaseServiceDeliveryOrganisationTypeViewModelValidator()
        {
            RuleFor(x => x.ServiceDeliveryOrganisationName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.ServiceDeliveryOrganisationName);

            RuleFor(x => x.ServiceDeliveryOrganisationNotes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.StandardNotes);
        }
    }
}