using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class SelectUserViewModelValidator : AbstractValidator<SelectUserViewModel>
    {
        public SelectUserViewModelValidator()
        {
            RuleFor(x => x.SelectedEmail)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(WebResources.UserPleaseSelect)
                .Length(0, 256);
        }
    }
}