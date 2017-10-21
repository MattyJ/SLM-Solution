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
    public class EditServiceComponentChildViewModelValidatorTests
    {
        private EditServiceComponentChildViewModelValidator _validator;
        private int _alternativeNameLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new EditServiceComponentChildViewModelValidator();
            _alternativeNameLength = typeof(ServiceComponent)
                .GetPropertyAttribute<StringLengthAttribute>("ComponentName")
                .MaximumLength;
        }

        [TestMethod]
        public void EditServiceComponentChildViewModelValidator_Validate_ComponentNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentChildViewModel()
                {
                    ComponentName = new string('X', _alternativeNameLength + 1)
                });
        }

        [TestMethod]
        public void EditServiceComponentChildViewModelValidator_Validate_ComponentNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentChildViewModel()
                {
                    ComponentName = new string('X', _alternativeNameLength)
                });
        }
    }
}