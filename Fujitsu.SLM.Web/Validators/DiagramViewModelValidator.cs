using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class DiagramViewModelValidator : AbstractValidator<DiagramViewModel>
    {
        public DiagramViewModelValidator()
        {
            RuleFor(x => x.DiagramNotes)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.StandardNotes);

            RuleFor(x => x.Filename)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.Filename);
        }
    }
}