using System.Collections.Generic;
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
    public class ServiceDeskViewModelValidatorTests
    {
        private ServiceDeskViewModelValidator _validator;
        private int _deskNameFieldLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ServiceDeskViewModelValidator();
            _deskNameFieldLength = typeof(ServiceDesk)
                .GetPropertyAttribute<StringLengthAttribute>("DeskName")
                .MaximumLength;
        }

        [TestMethod]
        public void ServiceDeskViewModelValidator_Validate_DeskNameIsNull_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.DeskName, new ServiceDeskViewModel());
        }

        [TestMethod]
        public void ServiceDeskViewModelValidator_Validate_CustomerIdZero_IsValidationError()
        {
            _validator.ShouldHaveValidationErrorFor(model => model.CustomerId, new ServiceDeskViewModel());
        }

        //[TestMethod]
        //public void ServiceDeskViewModelValidator_Validate_DeskInputTypesNull_IsValidationError()
        //{
        //    _validator.ShouldHaveValidationErrorFor(model => model.DeskInputTypes, new ServiceDeskViewModel());
        //}

        //[TestMethod]
        //public void ServiceDeskViewModelValidator_Validate_DeskInputTypesEmptyList_IsValidationError()
        //{
        //    _validator.ShouldHaveValidationErrorFor(model => model.DeskInputTypes,
        //        new ServiceDeskViewModel {DeskInputTypes = new List<InputTypeRefData>()});
        //}

        [TestMethod]
        public void ServiceDeskViewModelValidator_Validate_DeskNameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.DeskName,
                new ServiceDeskViewModel
                {
                    DeskName = new string('X', _deskNameFieldLength + 1)
                });
        }

        [TestMethod]
        public void ServiceDeskViewModelValidator_Validate_DeskNameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.DeskName,
                new ServiceDeskViewModel
                {
                    DeskName = new string('X', _deskNameFieldLength)
                });
        }
    }
}