using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class OperationalProcessTypeRefDataViewModelValidator : AbstractValidator<OperationalProcessTypeRefDataViewModel>
    {
        public OperationalProcessTypeRefDataViewModelValidator()
        {
            RuleFor(x => x.OperationalProcessTypeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.OperationalProcessName);

            RuleFor(x => x.Visible)
                .NotEqual(false).When(m => m.Standard).WithMessage("A Standard Operational Process Type must always be visible across all Customers");

            //RuleFor(x => x.Standard)
            //    .Equal(true).When(m => m.Visible == false).WithMessage("A Standard Operational Process Type must always be visible across all Customers");

            RuleFor(x => x.SortOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.SortOrder);
        }
    }
}