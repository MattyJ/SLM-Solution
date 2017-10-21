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
    public class EditServiceComponentNameViewModelValidatorTests
    {
        private EditServiceComponentNameViewModelValidator _validator;
        private int _componentNameLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new EditServiceComponentNameViewModelValidator();
            _componentNameLength = typeof(ServiceComponent)
                .GetPropertyAttribute<StringLengthAttribute>("ComponentName")
                .MaximumLength;
        }

        [TestMethod]
        public void EditServiceComponentNameViewModelValidator_Validate_ComponentNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentNameViewModel()
                {
                    ComponentName = new string('X', _componentNameLength + 1)
                });
        }

        [TestMethod]
        public void EditServiceComponentNameViewModelValidator_Validate_ComponentNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentNameViewModel()
                {
                    ComponentName = new string('X', _componentNameLength)
                });
        }

        [TestMethod]
        public void EditServiceComponentNameViewModelValidator_Validate_ComponentNameNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentNameViewModel()
                {
                    ComponentName = null
                });
        }

        [TestMethod]
        public void EditServiceComponentNameViewModelValidator_Validate_ComponentNameEmpty_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentNameViewModel()
                {
                    ComponentName = string.Empty
                });
        }
    }
}