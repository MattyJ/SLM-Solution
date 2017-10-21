using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

namespace Fujitsu.SLM.Web.Validators
{
    public class ServiceFunctionViewModelValidator : AbstractValidator<ServiceFunctionViewModel>
    {
        public ServiceFunctionViewModelValidator()
        {
            RuleFor(x => x.ServiceDomainId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(WebResources.ServiceDomainPleaseSelect)
                .GreaterThan(0)
                .WithMessage(WebResources.ServiceDomainPleaseSelect);

            RuleFor(x => x.FunctionTypeId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithMessage(WebResources.FunctionTypePleaseSelect)
                .GreaterThan(0)
                .WithMessage(WebResources.FunctionTypePleaseSelect);

            RuleFor(x => x.AlternativeName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Length(0, Validation.MaxLength.FunctionName);

            RuleFor(x => x.DiagramOrder)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .InclusiveBetween(1, Validation.MaxValue.DiagramOrder);
        }
    }
}