using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class AuditViewModelValidator : AbstractValidator<AuditViewModel>
    {
        public AuditViewModelValidator()
        {
            RuleFor(x => x.Version)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(1);

            RuleFor(x => x.ReasonForIssue)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(0, Validation.MaxLength.ReasonForIssue);

            RuleFor(x => x.Notes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.StandardNotes);
        }
    }
}