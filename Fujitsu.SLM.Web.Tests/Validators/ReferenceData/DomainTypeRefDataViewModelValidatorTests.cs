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
    public class DomainTypeRefDataViewModelValidatorTests
    {
        private DomainTypeRefDataViewModelValidator _validator;
        private int _domainNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new DomainTypeRefDataViewModelValidator();
            _domainNameFieldLength = typeof(DomainTypeRefData)
                .GetPropertyAttribute<StringLengthAttribute>("DomainName")
                .MaximumLength;
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_DomainNameNull_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.DomainName,
                new DomainTypeRefDataViewModel()
                {
                    DomainName = null
                });
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_DomainNameEmpty_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.DomainName,
                new DomainTypeRefDataViewModel()
                {
                    DomainName = string.Empty
                });
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_DomainNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.DomainName,
                new DomainTypeRefDataViewModel()
                {
                    DomainName = new string('X', _domainNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_DomainNameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.DomainName,
                new DomainTypeRefDataViewModel()
                {
                    DomainName = new string('X', _domainNameFieldLength)
                });
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_SortOrderIs1000_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.SortOrder,
                new DomainTypeRefDataViewModel()
                {
                    SortOrder = 1000
                });
        }

        [TestMethod]
        public void DomainTypeRefDataViewModelValidator_Validate_SortOrderGreaterThan1000_ValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.SortOrder,
                new DomainTypeRefDataViewModel()
                {
                    SortOrder = 1001
                });
        }
    }
}
