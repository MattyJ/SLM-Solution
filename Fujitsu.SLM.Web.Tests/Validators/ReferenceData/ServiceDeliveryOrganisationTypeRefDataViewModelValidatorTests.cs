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
    public class ServiceDeliveryOrganisationTypeRefDataViewModelValidatorTests
    {
        private ServiceDeliveryOrganisationTypeRefDataViewModelValidator _validator;
        private int _serviceDeliveryOrganisationTypeNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ServiceDeliveryOrganisationTypeRefDataViewModelValidator();
            _serviceDeliveryOrganisationTypeNameFieldLength = typeof(ServiceDeliveryOrganisationTypeRefData)
                .GetPropertyAttribute<StringLengthAttribute>("ServiceDeliveryOrganisationTypeName")
                .MaximumLength;
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataViewModelValidator_Validate_ResolverGroupTypeNameNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceDeliveryOrganisationTypeName,
                new ServiceDeliveryOrganisationTypeRefDataViewModel
                {
                    ServiceDeliveryOrganisationTypeName = null
                });
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataViewModelValidator_Validate_ResolverGroupTypeNameEmpty_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceDeliveryOrganisationTypeName,
                new ServiceDeliveryOrganisationTypeRefDataViewModel
                {
                    ServiceDeliveryOrganisationTypeName = string.Empty
                });
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataViewModelValidatorr_Validate_ResolverGroupTypeNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceDeliveryOrganisationTypeName,
                new ServiceDeliveryOrganisationTypeRefDataViewModel
                {
                    ServiceDeliveryOrganisationTypeName = new string('X', _serviceDeliveryOrganisationTypeNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataViewModelValidatorr_Validate_ResolverGroupTypeNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.ServiceDeliveryOrganisationTypeName,
                new ServiceDeliveryOrganisationTypeRefDataViewModel
                {
                    ServiceDeliveryOrganisationTypeName = new string('X', _serviceDeliveryOrganisationTypeNameFieldLength)
                });
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataViewModelValidatorr_Validate_SortOrderIs1000_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.SortOrder,
                new ServiceDeliveryOrganisationTypeRefDataViewModel
                {
                    SortOrder = 1000
                });
        }

        [TestMethod]
        public void ServiceDeliveryOrganisationTypeRefDataViewModelValidatorr_Validate_SortOrderGreaterThan1000_ValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.SortOrder,
                new ServiceDeliveryOrganisationTypeRefDataViewModel
                {
                    SortOrder = 1001
                });
        }
    }
}
