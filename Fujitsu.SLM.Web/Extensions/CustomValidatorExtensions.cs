using System.Collections.Generic;
using FluentValidation;

namespace Fujitsu.SLM.Web.Extensions
{
    public static class CustomValidatorExtensions
    {
        public static IRuleBuilderOptions<T, string> Contains<T>(this IRuleBuilder<T, string> ruleBuilder, IList<string> list)
        {
            return ruleBuilder.Must(list.Contains);
        }
    }
}