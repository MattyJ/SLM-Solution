using FluentValidation;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class AddTemplateServiceDomainViewModelValidator : AbstractValidator<AddTemplateServiceDomainViewModel>
    {
        public AddTemplateServiceDomainViewModelValidator()
        {
            RuleFor(x => x.ServiceDeskId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(WebResources.ServiceDeskPleaseSelect)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceDeskPleaseSelect);
        }
    }
}