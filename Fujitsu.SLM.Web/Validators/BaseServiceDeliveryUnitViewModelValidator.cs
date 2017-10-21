using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    class BaseServiceDeliveryUnitViewModelValidator : AbstractValidator<BaseServiceDeliveryUnitViewModel>
    {
        public BaseServiceDeliveryUnitViewModelValidator()
        {
            //this.RuleFor(x => x.ServiceDeliveryUnitName)
            //    .Cascade(CascadeMode.StopOnFirstFailure)
            //    .Length(0, 100);

            RuleFor(x => x.ServiceDeliveryUnitNotes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.StandardNotes);
        }
    }
}