using FluentValidation;
using Fujitsu.SLM.Web.Models;

namespace Fujitsu.SLM.Web.Validators
{
    public class BaseResolverGroupViewModelValidator : AbstractValidator<BaseResolverGroupViewModel>
    {
        public BaseResolverGroupViewModelValidator()
        {
                //RuleFor(x => x.ResolverName)
                //.Cascade(CascadeMode.StopOnFirstFailure)
                //.Length(0, 100);
        }
    }
}