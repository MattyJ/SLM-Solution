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
    public class ResolverGroupTypeRefDataViewModelValidatorTests
    {
        private ResolverGroupTypeRefDataViewModelValidator _validator;
        private int _resolverGroupTypeNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ResolverGroupTypeRefDataViewModelValidator();
            _resolverGroupTypeNameFieldLength = typeof(ResolverGroupTypeRefData)
                .GetPropertyAttribute<StringLengthAttribute>("ResolverGroupTypeName")
                .MaximumLength;
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataViewModelValidator_Validate_ResolverGroupTypeNameNull_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ResolverGroupTypeName,
                new ResolverGroupTypeRefDataViewModel
                {
                    ResolverGroupTypeName = null
                });
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataViewModelValidator_Validate_ResolverGroupTypeNameEmpty_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ResolverGroupTypeName,
                new ResolverGroupTypeRefDataViewModel
                {
                    ResolverGroupTypeName = string.Empty
                });
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataViewModelValidatorr_Validate_ResolverGroupTypeNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ResolverGroupTypeName,
                new ResolverGroupTypeRefDataViewModel
                {
                    ResolverGroupTypeName = new string('X', _resolverGroupTypeNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataViewModelValidatorr_Validate_ResolverGroupTypeNameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.ResolverGroupTypeName,
                new ResolverGroupTypeRefDataViewModel
                {
                    ResolverGroupTypeName = new string('X', _resolverGroupTypeNameFieldLength)
                });
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataViewModelValidatorr_Validate_SortOrderIs1000_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.Order,
                new ResolverGroupTypeRefDataViewModel
                {
                    Order = 1000
                });
        }

        [TestMethod]
        public void ResolverGroupTypeRefDataViewModelValidatorr_Validate_SortOrderGreaterThan1000_ValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.Order,
                new ResolverGroupTypeRefDataViewModel
                {
                    Order = 1001
                });
        }
    }
}
