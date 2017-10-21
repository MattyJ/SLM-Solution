using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class EditServiceComponentDiagramOrderViewModelValidator : AbstractValidator<EditServiceComponentDiagramOrderViewModel>
    {
        public EditServiceComponentDiagramOrderViewModelValidator()
        {
            RuleFor(x => x.DiagramOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .InclusiveBetween(1, Validation.MaxValue.DiagramOrder);
        }
    }
}
