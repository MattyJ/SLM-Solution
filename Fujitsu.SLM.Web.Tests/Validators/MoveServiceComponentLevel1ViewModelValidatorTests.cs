using FluentValidation.TestHelper;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class MoveServiceComponentLevel1ViewModelValidatorTests
    {
        private MoveServiceComponentLevel1ViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new MoveServiceComponentLevel1ViewModelValidator();
        }

        [TestMethod]
        public void MoveServiceComponentLevel1ViewModelValidator_Validate_DestinationServiceFunctionIdIsZero_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DestinationServiceFunctionId, new MoveServiceComponentLevel1ViewModel());
        }

        [TestMethod]
        public void MoveServiceComponentLevel1ViewModelValidator_Validate_ServiceComponentIdIsZero_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceComponentId, new MoveServiceComponentLevel1ViewModel());
        }
    }
}