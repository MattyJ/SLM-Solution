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
    public class CustomerViewModelValidatorTests
    {
        private CustomerViewModelValidator _validator;
        private int _customerNameLength;
        private int _customerNotesLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new CustomerViewModelValidator();
            _customerNameLength = typeof(Customer)
                .GetPropertyAttribute<StringLengthAttribute>("CustomerName")
                .MaximumLength;
            _customerNotesLength = typeof(Customer)
                .GetPropertyAttribute<StringLengthAttribute>("CustomerNotes")
                .MaximumLength;
        }

        [TestMethod]
        public void CustomerViewModelValidator_Validate_CustomerNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.CustomerName,
                new CustomerViewModel
                {
                    CustomerName = new string('X', _customerNameLength + 1)
                });
        }

        [TestMethod]
        public void CustomerViewModelValidator_Validate_CustomerNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.CustomerName,
                new CustomerViewModel
                {
                    CustomerName = new string('X', _customerNameLength)
                });
        }

        [TestMethod]
        public void CustomerViewModelValidator_Validate_CustomerNameCannotBeEmpty_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.CustomerName,
                new CustomerViewModel
                {
                    CustomerName = null
                });
        }

        [TestMethod]
        public void CustomerViewModelValidator_Validate_CustomerNotesIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.CustomerNotes,
                new CustomerViewModel
                {
                    CustomerNotes = new string('X', _customerNotesLength + 1)
                });
        }

        [TestMethod]
        public void CustomerViewModelValidator_Validate_CustomerNotesIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.CustomerNotes,
                new CustomerViewModel
                {
                    CustomerNotes = new string('X', _customerNotesLength)
                });
        }

        [TestMethod]
        public void CustomerViewModelValidator_Validate_CustomerNotesCanBeEmpty_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.CustomerNotes,
                new CustomerViewModel
                {
                    CustomerNotes = null
                });
        }

        [TestMethod]
        public void CustomerViewModelValidator_Validate_AssignedArchitectCannotBeEmpty_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.AssignedArchitect,
                new CustomerViewModel
                {
                    CustomerName = "3663",
                    AssignedArchitect = null
                });
        }
    }
}