using FluentValidation.TestHelper;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class MoveResolverViewModelValidatorTests
    {
        private MoveResolverViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new MoveResolverViewModelValidator();
        }

        [TestMethod]
        public void MoveResolverViewModelValidator_Validate_DestinationServiceComponentIdIsZero_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DestinationServiceComponentId, new MoveResolverViewModel());
        }

        [TestMethod]
        public void MoveResolverViewModelValidator_Validate_ServiceComponentIdIsZero_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceComponentId, new MoveResolverViewModel());
        }
    }
}