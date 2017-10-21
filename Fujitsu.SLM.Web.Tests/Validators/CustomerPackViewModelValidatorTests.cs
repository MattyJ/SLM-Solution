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
    public class CustomerPackViewModelValidatorTests
    {
        private CustomerPackViewModelValidator _validator;
        private int _filenameFieldLength;
        private int _packNotes;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new CustomerPackViewModelValidator();
            _filenameFieldLength = typeof(CustomerPack)
                .GetPropertyAttribute<StringLengthAttribute>("Filename")
                .MaximumLength;
            _packNotes = typeof(CustomerPack)
                .GetPropertyAttribute<StringLengthAttribute>("PackNotes")
                .MaximumLength;
        }

        [TestMethod]
        public void CustomerPackViewModelValidator_Validate_FilenameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.Filename,
                new CustomerPackViewModel()
                {
                    Filename = new string('X', _filenameFieldLength + 1)
                });
        }

        [TestMethod]
        public void CustomerPackViewModelValidator_Validate_FilenameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.Filename,
                new CustomerPackViewModel()
                {
                    Filename = new string('X', _filenameFieldLength)
                });
        }

        [TestMethod]
        public void CustomerPackViewModelValidator_Validate_FilenameCannotBeEmpty_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.Filename,
                new CustomerPackViewModel()
                {
                    Filename = null
                });
        }

        [TestMethod]
        public void CustomerPackViewModelValidator_Validate_PackNotesIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.PackNotes,
                new CustomerPackViewModel()
                {
                    PackNotes = new string('X', _packNotes + 1)
                });
        }

        [TestMethod]
        public void CustomerPackViewModelValidator_Validate_PackNotesIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.PackNotes,
                new CustomerPackViewModel()
                {
                    PackNotes = new string('X', _packNotes)
                });
        }

        [TestMethod]
        public void CustomerPackViewModelValidator_Validate_PackNotesCanBeEmpty_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.PackNotes,
                new CustomerPackViewModel()
                {
                    PackNotes = null
                });
        }
    }
}