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
    public class EditServiceComponentNameLevelViewModelValidatorTests
    {
        private EditServiceComponentNameLevelViewModelValidator _validator;
        private int _componentNameLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new EditServiceComponentNameLevelViewModelValidator();
            _componentNameLength = typeof(ServiceComponent)
                .GetPropertyAttribute<StringLengthAttribute>("ComponentName")
                .MaximumLength;
        }

        [TestMethod]
        public void EditServiceComponentNameLevelViewModelValidator_Validate_ComponentNameIsNull_IsValidationError()
        {
            var vm = new EditServiceComponentNameLevelViewModel
            {
                ComponentName = null,
                ComponentLevel = "Level1",
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void EEditServiceComponentNameLevelViewModelValidator_Validate_ComponentNameIsEmpty_IsValidationError()
        {
            var vm = new EditServiceComponentNameLevelViewModel
            {
                ComponentName = string.Empty,
                ComponentLevel = "Level1",
            };
            var result = _validator.Validate(vm);
            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void EditServiceComponentNameLevelViewModelValidator_Validate_ComponentNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentNameLevelViewModel
                {
                    ComponentName = new string('X', _componentNameLength + 1)
                });
        }

        [TestMethod]
        public void EditServiceComponentNameLevelViewModelValidator_Validate_ComponentNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.ComponentName,
                new EditServiceComponentNameLevelViewModel()
                {
                    ComponentName = new string('X', _componentNameLength)
                });
        }

        [TestMethod]
        public void EditServiceComponentNameLevelViewModelValidator_Validate_ComponentLevelNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentLevel,
                new EditServiceComponentNameLevelViewModel()
                {
                    ComponentLevel = null
                });
        }

        [TestMethod]
        public void EditServiceComponentNameLevelViewModelValidator_Validate_ComponentLevelEmptyString_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentLevel,
                new EditServiceComponentNameLevelViewModel()
                {
                    ComponentLevel = string.Empty
                });
        }

        [TestMethod]
        public void EditServiceComponentNameLevelViewModelValidator_Validate_ComponentLevelIncorrect_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ComponentLevel,
                new EditServiceComponentNameLevelViewModel()
                {
                    ComponentLevel = "XXX"
                });
        }

        [TestMethod]
        public void EditServiceComponentNameLevelViewModelValidator_Validate_ComponentLevelCorrect_IsNotValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.ComponentLevel,
                new EditServiceComponentNameLevelViewModel()
                {
                    ComponentLevel = "Level1"
                });
        }
    }
}