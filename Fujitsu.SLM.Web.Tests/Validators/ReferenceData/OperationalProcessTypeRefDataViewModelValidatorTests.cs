using FluentValidation.TestHelper;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class OperationalProcessTypeRefDataViewModelValidatorTests
    {
        private OperationalProcessTypeRefDataViewModelValidator _validator;
        private int _operationalProcessTypeNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new OperationalProcessTypeRefDataViewModelValidator();
            _operationalProcessTypeNameFieldLength = typeof(OperationalProcessTypeRefData)
                .GetPropertyAttribute<StringLengthAttribute>("OperationalProcessTypeName")
                .MaximumLength;
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidator_Validate_OperationalProcessTypeNameNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.OperationalProcessTypeName,
                new OperationalProcessTypeRefDataViewModel
                {
                    OperationalProcessTypeName = null
                });
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidator_Validate_OperationalProcessTypeNameEmpty_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.OperationalProcessTypeName,
                new OperationalProcessTypeRefDataViewModel
                {
                    OperationalProcessTypeName = string.Empty
                });
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidatorr_Validate_OperationalProcessTypeNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.OperationalProcessTypeName,
                new OperationalProcessTypeRefDataViewModel
                {
                    OperationalProcessTypeName = new string('X', _operationalProcessTypeNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidatorr_Validate_OperationalProcessTypeNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.OperationalProcessTypeName,
                new OperationalProcessTypeRefDataViewModel
                {
                    OperationalProcessTypeName = new string('X', _operationalProcessTypeNameFieldLength)
                });
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidatorr_Validate_SortOrderIs1000_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.SortOrder,
                new OperationalProcessTypeRefDataViewModel
                {
                    SortOrder = 1000
                });
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidatorr_Validate_SortOrderGreaterThan1000_ValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.SortOrder,
                new OperationalProcessTypeRefDataViewModel
                {
                    SortOrder = 1001
                });
        }
        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidatorr_Validate_StandardTypeMustBeVisible_ValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.Visible,
                new OperationalProcessTypeRefDataViewModel
                {
                    Visible = false,
                    Standard = true
                });
        }

        [TestMethod]
        public void OperationalProcessTypeRefDataViewModelValidatorr_Validate_StandardOperationalProcessTypeIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.OperationalProcessTypeName,
                new OperationalProcessTypeRefDataViewModel
                {
                    OperationalProcessTypeName = new string('X', _operationalProcessTypeNameFieldLength),
                    Visible = true,
                    Standard = true
                });
        }
    }
}
