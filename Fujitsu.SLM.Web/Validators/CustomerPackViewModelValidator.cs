using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class CustomerPackViewModelValidator : AbstractValidator<CustomerPackViewModel>
    {
        public CustomerPackViewModelValidator()
        {
            RuleFor(x => x.Filename)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.Filename);

            RuleFor(x => x.PackNotes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.StandardNotes);
        }
    }
}