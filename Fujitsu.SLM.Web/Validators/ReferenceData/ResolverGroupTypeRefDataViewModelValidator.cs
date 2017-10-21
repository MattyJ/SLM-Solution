using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class ResolverGroupTypeRefDataViewModelValidator : AbstractValidator<ResolverGroupTypeRefDataViewModel>
    {
        public ResolverGroupTypeRefDataViewModelValidator()
        {
            RuleFor(x => x.ResolverGroupTypeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.ResolverGroupName);

            RuleFor(x => x.Order)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.SortOrder);
        }
    }
}