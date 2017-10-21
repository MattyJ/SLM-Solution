using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class BaseServiceDeliveryOrganisationViewModelValidator : AbstractValidator<BaseServiceDeliveryOrganisationViewModel>
    {
        public BaseServiceDeliveryOrganisationViewModelValidator()
        {
            //RuleFor(x => x.ServiceDeliveryOrganisationName)
            //    .Cascade(CascadeMode.StopOnFirstFailure)
            //    .Length(0, 100);

            RuleFor(x => x.ServiceDeliveryOrganisationNotes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.StandardNotes);
        }
    }
}