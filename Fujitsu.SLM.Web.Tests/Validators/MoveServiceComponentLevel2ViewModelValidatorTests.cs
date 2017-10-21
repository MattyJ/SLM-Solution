using FluentValidation.TestHelper;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class MoveServiceComponentLevel2ViewModelValidatorTests
    {
        private MoveServiceComponentLevel2ViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new MoveServiceComponentLevel2ViewModelValidator();
        }

        [TestMethod]
        public void MoveServiceComponentLevel2ViewModelValidator_Validate_DestinationServiceFunctionIdIsZero_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DestinationServiceComponentId, new MoveServiceComponentLevel2ViewModel());
        }

        [TestMethod]
        public void MoveServiceComponentLevel2ViewModelValidator_Validate_ServiceComponentIdIsZero_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceComponentId, new MoveServiceComponentLevel2ViewModel());
        }
    }
}