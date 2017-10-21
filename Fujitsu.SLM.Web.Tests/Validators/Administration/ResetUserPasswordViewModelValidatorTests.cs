using FluentValidation.TestHelper;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class ResetUserPasswordViewModelValidatorTests
    {
        private ResetUserPasswordViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ResetUserPasswordViewModelValidator();
        }

        [TestMethod]
        public void ResetUserPasswordViewModelValidator_Validate_UserId_ShouldBeNotNull()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.UserId, new ResetUserPasswordViewModel());
        }

        [TestMethod]
        public void ResetUserPasswordViewModelValidator_Validate_UserId_ShouldBeNotEmpty()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.UserId, new ResetUserPasswordViewModel {UserId = string.Empty});
        }
    }
}