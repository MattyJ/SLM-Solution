using System.Diagnostics.CodeAnalysis;
using FluentValidation.Mvc;

namespace Fujitsu.SLM.Web
{
    [ExcludeFromCodeCoverage]
    public static class ValidationConfig
    {
        public static void Configure()
        {
            FluentValidationModelValidatorProvider.Configure();
        }
    }
}
