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
    public class ContextHelpRefDataViewModelValidatorTests
    {
        private ContextHelpRefDataViewModelValidator _validator;
        private int _titleFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ContextHelpRefDataViewModelValidator();
            _titleFieldLength = typeof(ContextHelpRefData)
                .GetPropertyAttribute<StringLengthAttribute>("Title")
                .MaximumLength;
        }

        [TestMethod]
        public void ContextHelpRefDataViewModelValidator_Validate_TitleNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.Title,
                new ContextHelpRefDataViewModel
                {
                    Title = null
                });
        }

        [TestMethod]
        public void ContextHelpRefDataViewModelValidator_Validate_TitleEmpty_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.Title,
                new ContextHelpRefDataViewModel()
                {
                    Title = string.Empty
                });
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_DomainNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.Title,
                new ContextHelpRefDataViewModel()
                {
                    Title = new string('X', _titleFieldLength + 1)
                });
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_TitleIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.Title,
                new ContextHelpRefDataViewModel()
                {
                    Title = new string('X', _titleFieldLength)
                });
        }
    }
}
