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
    public class FunctionTypeRefDataViewModelValidatorTests
    {
        private FunctionTypeRefDataViewModelValidator _validator;
        private int _functionNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new FunctionTypeRefDataViewModelValidator();
            _functionNameFieldLength = typeof(FunctionTypeRefData)
                .GetPropertyAttribute<StringLengthAttribute>("FunctionName")
                .MaximumLength;
        }

        [TestMethod]
        public void FunctionTypeRefDataViewModelValidator_Validate_FunctionNameNull_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.FunctionName,
                new FunctionTypeRefDataViewModel()
                {
                    FunctionName = null
                });
        }

        [TestMethod]
        public void FunctionTypeRefDataViewModelValidator_Validate_DomainNameEmpty_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.FunctionName,
                new FunctionTypeRefDataViewModel()
                {
                    FunctionName = string.Empty
                });
        }

        [TestMethod]
        public void FunctionTypeRefDataViewModelValidatorr_Validate_DomainNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.FunctionName,
                new FunctionTypeRefDataViewModel()
                {
                    FunctionName = new string('X', _functionNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void FunctionTypeRefDataViewModelValidatorr_Validate_DomainNameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.FunctionName,
                new FunctionTypeRefDataViewModel()
                {
                    FunctionName = new string('X', _functionNameFieldLength)
                });
        }

        [TestMethod]
        public void FunctionTypeRefDataViewModelValidatorr_Validate_SortOrderIs1000_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.SortOrder,
                new FunctionTypeRefDataViewModel()
                {
                    SortOrder = 1000
                });
        }

        [TestMethod]
        public void FunctionTypeRefDataViewModelValidatorr_Validate_SortOrderGreaterThan1000_ValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.SortOrder,
                new FunctionTypeRefDataViewModel()
                {
                    SortOrder = 1001
                });
        }
    }
}
