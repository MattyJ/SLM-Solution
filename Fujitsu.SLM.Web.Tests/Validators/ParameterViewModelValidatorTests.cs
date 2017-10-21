using System.ComponentModel.DataAnnotations;
using FluentValidation.TestHelper;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class ParameterViewModelValidatorTests
    {
        private ParameterViewModelValidator _validator;
        private int _parameterNameFieldLength;
        private int _parameterValueFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ParameterViewModelValidator();
            _parameterNameFieldLength = typeof(Parameter)
                .GetPropertyAttribute<StringLengthAttribute>("ParameterName")
                .MaximumLength;
            _parameterValueFieldLength = typeof(Parameter)
                .GetPropertyAttribute<StringLengthAttribute>("ParameterValue")
                .MaximumLength;
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterNameIsNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ParameterName, new ParameterViewModel());
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterNameIsEmptyString_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ParameterName,
                new ParameterViewModel {ParameterValue = string.Empty});
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ParameterName,
                new ParameterViewModel()
                {
                    ParameterName = new string('X', _parameterNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterNameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.ParameterName,
                new ParameterViewModel
                {
                    ParameterName = new string('X', _parameterNameFieldLength)
                });
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterValueIsNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ParameterName, new ParameterViewModel());
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterValueIsEmptyString_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ParameterName,
                new ParameterViewModel {ParameterValue = string.Empty});
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterValueIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ParameterValue,
                new ParameterViewModel()
                {
                    ParameterValue = new string('X', _parameterValueFieldLength + 1)
                });
        }

        [TestMethod]
        public void ParametersViewModelValidator_Validate_ParameterValueIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.ParameterValue,
                new ParameterViewModel
                {
                    ParameterValue = new string('X', _parameterValueFieldLength)
                });
        }
    }
}