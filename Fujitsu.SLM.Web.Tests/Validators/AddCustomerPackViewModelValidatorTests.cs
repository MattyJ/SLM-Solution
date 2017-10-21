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
    public class AddCustomerPackViewModelValidatorTests
    {
        private AddCustomerPackViewModelValidator _validator;
        private int _filenameFieldLength;
        private int _packNotes;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new AddCustomerPackViewModelValidator();
            _filenameFieldLength = typeof(CustomerPack)
                .GetPropertyAttribute<StringLengthAttribute>("Filename")
                .MaximumLength;
            _packNotes = typeof(CustomerPack)
                .GetPropertyAttribute<StringLengthAttribute>("PackNotes")
                .MaximumLength;
        }

        [TestMethod]
        public void AddCustomerPackViewModelValidator_Validate_FilenameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.Filename,
                new AddCustomerPackViewModel()
                {
                    Filename = new string('X', _filenameFieldLength + 1)
                });
        }

        [TestMethod]
        public void AddCustomerPackViewModelValidator_Validate_FilenameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.Filename,
                new AddCustomerPackViewModel()
                {
                    Filename = new string('X', _filenameFieldLength)
                });
        }

        [TestMethod]
        public void AddCustomerPackViewModelValidator_Validate_FileNameCannotBeEmpty_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.Filename,
                new AddCustomerPackViewModel
                {
                    Filename = null
                });
        }

        [TestMethod]
        public void AddCustomerPackViewModelValidator_Validate_PackNotesIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.PackNotes,
                new AddCustomerPackViewModel
                {
                    PackNotes = new string('X', _packNotes + 1)
                });
        }

        [TestMethod]
        public void AddCustomerPackViewModelValidator_Validate_PackNotesIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.PackNotes,
                new AddCustomerPackViewModel
                {
                    PackNotes = new string('X', _packNotes)
                });
        }

        [TestMethod]
        public void AddCustomerPackViewModelValidator_Validate_PackNotesCanBeEmpty_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.PackNotes,
                new AddCustomerPackViewModel
                {
                    PackNotes = null
                });
        }
    }
}