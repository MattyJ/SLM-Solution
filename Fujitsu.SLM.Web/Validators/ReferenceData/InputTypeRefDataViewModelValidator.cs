using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class InputTypeRefDataViewModelValidator : AbstractValidator<InputTypeRefDataViewModel>
    {
        public InputTypeRefDataViewModelValidator()
        {
            RuleFor(x => x.InputTypeNumber)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.InputNumber);

            RuleFor(x => x.InputTypeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.InputName);

            RuleFor(x => x.SortOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.SortOrder);
        }
    }
}
