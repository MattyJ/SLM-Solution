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
    public class ServiceDeliveryUnitTypeRefDataViewModelValidatorTests
    {
        private ServiceDeliveryUnitTypeRefDataViewModelValidator _validator;
        private int _resolverGroupTypeNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ServiceDeliveryUnitTypeRefDataViewModelValidator();
            _resolverGroupTypeNameFieldLength = typeof(ServiceDeliveryUnitTypeRefData)
                .GetPropertyAttribute<StringLengthAttribute>("ServiceDeliveryUnitTypeName")
                .MaximumLength;
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataViewModelValidator_Validate_ResolverGroupTypeNameNull_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ServiceDeliveryUnitTypeName,
                new ServiceDeliveryUnitTypeRefDataViewModel
                {
                    ServiceDeliveryUnitTypeName = null
                });
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataViewModelValidator_Validate_ResolverGroupTypeNameEmpty_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ServiceDeliveryUnitTypeName,
                new ServiceDeliveryUnitTypeRefDataViewModel()
                {
                    ServiceDeliveryUnitTypeName = string.Empty
                });
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataViewModelValidatorr_Validate_ResolverGroupTypeNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.ServiceDeliveryUnitTypeName,
                new ServiceDeliveryUnitTypeRefDataViewModel()
                {
                    ServiceDeliveryUnitTypeName = new string('X', _resolverGroupTypeNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataViewModelValidatorr_Validate_ResolverGroupTypeNameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.ServiceDeliveryUnitTypeName,
                new ServiceDeliveryUnitTypeRefDataViewModel()
                {
                    ServiceDeliveryUnitTypeName = new string('X', _resolverGroupTypeNameFieldLength)
                });
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataViewModelValidatorr_Validate_SortOrderIs1000_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.SortOrder,
                new ServiceDeliveryUnitTypeRefDataViewModel()
                {
                    SortOrder = 1000
                });
        }

        [TestMethod]
        public void ServiceDeliveryUnitTypeRefDataViewModelValidatorr_Validate_SortOrderGreaterThan1000_ValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.SortOrder,
                new ServiceDeliveryUnitTypeRefDataViewModel()
                {
                    SortOrder = 1001
                });
        }
    }
}
