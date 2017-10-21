using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class RegionTypeRefDataViewModelValidator : AbstractValidator<RegionTypeRefDataViewModel>
    {
        public RegionTypeRefDataViewModelValidator()
        {
            RuleFor(x => x.RegionTypeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.RegionName);

            RuleFor(x => x.SortOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, Validation.MaxValue.SortOrder);
        }
    }
}