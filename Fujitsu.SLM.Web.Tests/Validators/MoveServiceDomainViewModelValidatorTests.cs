using FluentValidation.TestHelper;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class MoveServiceDomainViewModelValidatorTests
    {
        private MoveServiceDomainViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new MoveServiceDomainViewModelValidator();
        }

        [TestMethod]
        public void MoveServiceDomainViewModelValidator_Validate_ServiceDeskIdIsNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceDeskId, new MoveServiceDomainViewModel());
        }

        [TestMethod]
        public void MoveServiceDomainViewModelValidator_Validate_ServiceDomainIdIsNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceDomainId, new MoveServiceDomainViewModel());
        }
    }
}
