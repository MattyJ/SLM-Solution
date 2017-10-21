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
    public class ServiceFunctionViewModelValidatorTests
    {
        private ServiceFunctionViewModelValidator _validator;
        private int _alternativeNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ServiceFunctionViewModelValidator();
            _alternativeNameFieldLength = typeof(ServiceFunction)
                .GetPropertyAttribute<StringLengthAttribute>("AlternativeName")
                .MaximumLength;
        }

        [TestMethod]
        public void ServiceFunctionViewModelValidator_Validate_AlternativeNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.AlternativeName,
                new ServiceFunctionViewModel
                {
                    AlternativeName = new string('X', _alternativeNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void ServiceFunctionViewModelValidator_Validate_AlternativeNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.AlternativeName,
                new ServiceFunctionViewModel
                {
                    AlternativeName = new string('X', _alternativeNameFieldLength)
                });
        }

        [TestMethod]
        public void ServiceFunctionViewModelValidator_Validate_DiagramOrderIsWithinRangeIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.DiagramOrder,
                new ServiceFunctionViewModel
                {
                    DiagramOrder = 5
                });
        }

        [TestMethod]
        public void ServiceFunctionViewModelValidator_Validate_DiagramOrderIsLessThanValidRange_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DiagramOrder,
                new ServiceFunctionViewModel
                {
                    DiagramOrder = 0
                });
        }

        [TestMethod]
        public void ServiceFunctionViewModelValidator_Validate_DiagramOrderIsGreaterThanValidRange_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DiagramOrder,
                new ServiceFunctionViewModel
                {
                    DiagramOrder = Validation.MaxValue.DiagramOrder + 1
                });
        }
    }
}
