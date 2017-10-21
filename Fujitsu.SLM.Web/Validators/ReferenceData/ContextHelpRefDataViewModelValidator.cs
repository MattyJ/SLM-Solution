using FluentValidation;
using Fujitsu.SLM.Web.Extensions;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using System.Collections.Generic;

namespace Fujitsu.SLM.Web.Validators
{
    public class ContextHelpRefDataViewModelValidator : AbstractValidator<ContextHelpRefDataViewModel>
    {
        public ContextHelpRefDataViewModelValidator()
        {
            var validContentTypes = new List<string>
            {
                "video/mp4"
            };

            RuleFor(x => x.Title)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .Length(1, 100);

            RuleFor(x => x.HelpVideoFile.ContentType)
                .Contains(validContentTypes).When(w => w.HelpVideoFile != null).WithMessage(WebResources.InvalidContentType);
        }
    }
}
