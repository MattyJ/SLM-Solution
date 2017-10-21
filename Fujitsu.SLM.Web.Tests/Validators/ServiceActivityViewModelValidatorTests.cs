using FluentValidation.TestHelper;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class ServiceActivityViewModelValidatorTests
    {
        private ServiceActivityViewModelValidator _validator;
        private int _activityNameLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ServiceActivityViewModelValidator();
            _activityNameLength = typeof(ServiceComponent)
                .GetPropertyAttribute<StringLengthAttribute>("ServiceActivities")
                .MaximumLength;
        }

        [TestMethod]
        public void ServiceActivityViewModelValidator_Validate_ActivityNameIsTooLong_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.ServiceActivities,
                new ServiceActivityViewModel
                {
                    ServiceActivities = new string('X', _activityNameLength + 1)
                });
        }

        [TestMethod]
        public void ServiceActivityViewModelValidator_Validate_ActivityNameIsOk_NoValidationError()
        {
            _validator.ShouldNotHaveValidationErrorFor(model => model.ServiceActivities,
                new ServiceActivityViewModel
                {
                    ServiceActivities = new string('X', _activityNameLength)
                });
        }
    }
}