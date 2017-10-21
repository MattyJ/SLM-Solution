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
    public class DiagramViewModelValidatorTests
    {
        private DiagramViewModelValidator _validator;
        private int _filenameFieldLength;
        private int _diagramNotes;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new DiagramViewModelValidator();
            _filenameFieldLength = typeof(CustomerPack)
                .GetPropertyAttribute<StringLengthAttribute>("Filename")
                .MaximumLength;
            _diagramNotes = typeof(CustomerPack)
                .GetPropertyAttribute<StringLengthAttribute>("PackNotes")
                .MaximumLength;
        }

        [TestMethod]
        public void DiagramViewModelValidator_Validate_FilenameIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.Filename,
                new DiagramViewModel()
                {
                    Filename = new string('X', _filenameFieldLength + 1)
                });
        }

        [TestMethod]
        public void DiagramViewModelValidator_Validate_FilenameIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.Filename,
                new DiagramViewModel()
                {
                    Filename = new string('X', _filenameFieldLength)
                });
        }

        [TestMethod]
        public void DiagramViewModelValidator_Validate_PackNotesIsTooLong_IsValidationError()
        {
            this._validator.ShouldHaveValidationErrorFor(model => model.DiagramNotes,
                new DiagramViewModel()
                {
                    DiagramNotes = new string('X', _diagramNotes + 1)
                });
        }

        [TestMethod]
        public void DiagramViewModelValidator_Validate_PackNotesIsOk_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.DiagramNotes,
                new DiagramViewModel()
                {
                    DiagramNotes = new string('X', _diagramNotes)
                });
        }

        [TestMethod]
        public void DiagramViewModelValidator_Validate_PackNotesCanBeEmpty_NoValidationError()
        {
            this._validator.ShouldNotHaveValidationErrorFor(model => model.DiagramNotes,
                new DiagramViewModel()
                {
                    DiagramNotes = null
                });
        }
    }
}