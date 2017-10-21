using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class ServiceDomainViewModelValidator : AbstractValidator<ServiceDomainViewModel>
    {
        public ServiceDomainViewModelValidator()
        {
            RuleFor(x => x.ServiceDeskId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(WebResources.ServiceDeskPleaseSelect)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceDeskPleaseSelect);

            RuleFor(x => x.DomainTypeId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(WebResources.DomainTypePleaseSelect)
                .GreaterThan(0)
                .WithMessage(WebResources.DomainTypePleaseSelect);

            RuleFor(x => x.AlternativeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.DomainName);

            RuleFor(x => x.DiagramOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .InclusiveBetween(1, Validation.MaxValue.DiagramOrder);
        }
    }
}