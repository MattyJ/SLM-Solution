using FluentValidation.TestHelper;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class RoleViewModelValidatorTests
    {
        private RoleViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new RoleViewModelValidator();
        }

        [TestMethod]
        public void RoleViewModelValidator_Validate_RoleNameIsNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.RoleName, new RoleViewModel());
        }

        [TestMethod]
        public void RoleViewModelValidator_Validate_UserIdIsNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.UserId, new RoleViewModel());
        }

        [TestMethod]
        public void RoleViewModelValidator_Validate_UserIdIsNull_ReturnsValidationErrorWithCorrectMessage()
        {
            var model = new RoleViewModel()
            {
                RoleName = "Administrator",
            };
            var result = this._validator.Validate(model);
            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.UserPleaseSelect, result.Errors[0].ErrorMessage);
        }
    }
}
