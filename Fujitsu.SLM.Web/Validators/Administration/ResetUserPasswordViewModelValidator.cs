using FluentValidation;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class ResetUserPasswordViewModelValidator : AbstractValidator<ResetUserPasswordViewModel>
    {
        public ResetUserPasswordViewModelValidator()
        {
            this.RuleFor(x => x.UserId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty();
        }
    }
}