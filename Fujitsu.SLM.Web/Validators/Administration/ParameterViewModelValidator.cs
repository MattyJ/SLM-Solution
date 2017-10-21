using FluentValidation;
using Fujitsu.SLM.Web.Models;

// ReSharper disable once CheckNamespace
namespace Fujitsu.SLM.Web.Validators
{
    public class ParameterViewModelValidator : AbstractValidator<ParameterViewModel>
    {
        public ParameterViewModelValidator()
        {
            this.RuleFor(x => x.ParameterName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, 50);

            this.RuleFor(x => x.ParameterValue)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, 200);
        }
    }
}