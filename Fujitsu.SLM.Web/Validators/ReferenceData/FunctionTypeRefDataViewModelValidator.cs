using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class FunctionTypeRefDataViewModelValidator : AbstractValidator<FunctionTypeRefDataViewModel>
    {
        public FunctionTypeRefDataViewModelValidator()
        {
            RuleFor(x => x.FunctionName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.FunctionName);

            RuleFor(x => x.SortOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.SortOrder);
        }
    }
}
