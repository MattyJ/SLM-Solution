using System.Collections.Generic;
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
    public class InputTypeRefDataViewModelValidatorTests
    {
        private InputTypeRefDataViewModelValidator _validator;
        private int _inputTypeNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new InputTypeRefDataViewModelValidator();
            _inputTypeNameFieldLength = typeof(InputTypeRefData)
                .GetPropertyAttribute<StringLengthAttribute>("InputTypeName")
                .MaximumLength;
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_InputTypeNumberIs1000_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.InputTypeNumber,
                new InputTypeRefDataViewModel()
                {
                    InputTypeNumber = 1000
                });
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_InputTypeNumberGreaterThan1000_ValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.InputTypeNumber,
                new InputTypeRefDataViewModel()
                {
                    InputTypeNumber = 1001
                });
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_InputTypeNameNull_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.InputTypeName,
                new InputTypeRefDataViewModel()
                {
                    InputTypeName = null
                });
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_InputTypeNameEmpty_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.InputTypeName,
                new InputTypeRefDataViewModel()
                {
                    InputTypeName = string.Empty
                });
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_InputTypeNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.InputTypeName,
                new InputTypeRefDataViewModel()
                {
                    InputTypeName = new string('X', _inputTypeNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_InputTypeNameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.InputTypeName,
                new InputTypeRefDataViewModel()
                {
                    InputTypeName = new string('X', _inputTypeNameFieldLength)
                });
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_SortOrderIs1000_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.SortOrder,
                new InputTypeRefDataViewModel()
                {
                    SortOrder = 1000
                });
        }

        [TestMethod]
        public void InputTypeRefDataViewModelValidator_Validate_SortOrderGreaterThan1000_ValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.SortOrder,
                new InputTypeRefDataViewModel()
                {
                    SortOrder = 1001
                });
        }
    }
}
