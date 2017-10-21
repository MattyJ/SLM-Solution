using FluentValidation.TestHelper;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class SelectUserViewModelValidatorTests
    {
        private SelectUserViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new SelectUserViewModelValidator();
        }

        [TestMethod]
        public void SelectUserViewModelValidator_Validate_SelectedEmailIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.SelectedEmail,
                new SelectUserViewModel()
                {
                    SelectedEmail = new string('X', 257)
                });
        }

        [TestMethod]
        public void SelectUserViewModelValidator_Validate_SelectedEmailIsOK_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.SelectedEmail,
                new SelectUserViewModel()
                {
                    SelectedEmail = new string('X', 256)
                });
        }

        [TestMethod]
        public void SelectUserViewModelValidator_Validate_SelectedEmailCannotBeEmpty_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.SelectedEmail,
                new SelectUserViewModel()
                {
                    SelectedEmail = string.Empty
                });
        }

        [TestMethod]
        public void SelectUserViewModelValidator_Validate_SelectedEmailCannotBeNull_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.SelectedEmail,
                new SelectUserViewModel()
                {
                    SelectedEmail = null
                });
        }
    }
}
