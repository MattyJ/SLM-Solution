using FluentValidation.TestHelper;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class ServiceDomainViewModelValidatorTests
    {
        private ServiceDomainViewModelValidator _validator;
        private int _alternativeNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ServiceDomainViewModelValidator();
            _alternativeNameFieldLength = typeof(ServiceDomain)
                .GetPropertyAttribute<StringLengthAttribute>("AlternativeName")
                .MaximumLength;
        }

        [TestMethod]
        public void ServiceDomainViewModelValidator_Validate_AlternativeNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.AlternativeName,
                new ServiceDomainViewModel
                {
                    AlternativeName = new string('X', _alternativeNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void ServiceDomainViewModelValidator_Validate_AlternativeNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.AlternativeName,
                new ServiceDomainViewModel
                {
                    AlternativeName = new string('X', _alternativeNameFieldLength)
                });
        }

        [TestMethod]
        public void ServiceDomainViewModelValidator_Validate_DiagramOrderIsWithinRangeIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.DiagramOrder,
                new ServiceDomainViewModel
                {
                    DiagramOrder = 5
                });
        }

        [TestMethod]
        public void ServiceDomainViewModelValidator_Validate_DiagramOrderIsLessThanValidRange_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DiagramOrder,
                new ServiceDomainViewModel
                {
                    DiagramOrder = 0
                });
        }

        [TestMethod]
        public void ServiceDomainViewModelValidator_Validate_DiagramOrderIsGreaterThanValidRange_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DiagramOrder,
                new ServiceDomainViewModel
                {
                    DiagramOrder = Validation.MaxValue.DiagramOrder + 1
                });
        }
    }
}
