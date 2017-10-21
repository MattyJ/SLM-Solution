using FluentValidation;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;

// ReSharper disable once CheckNamespace
namespace Fujitsu.SLM.Web.Validators
{
    public class ServiceDeskViewModelValidator : AbstractValidator<ServiceDeskViewModel>
    {
        public ServiceDeskViewModelValidator()
        {
            RuleFor(x => x.DeskName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, Validation.MaxLength.DeskName);

            RuleFor(x => x.CustomerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0)
                .WithMessage(WebResources.CustomerPleaseSelect);

            //this.RuleFor(x => x.DeskInputTypes)
            //    .Cascade(CascadeMode.StopOnFirstFailure)
            //    .Must(m => m != null && m.Count > 0)
            //    .WithMessage(WebResources.DeskInputTypesAtLeastOne);
        }
    }
}