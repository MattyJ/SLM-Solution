using FluentValidation.TestHelper;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Diagram = Fujitsu.SLM.Model.Diagram;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    public class UploadCustomerFileViewModelValidatorTests
    {
        private UploadCustomerFileViewModelValidator _validator;
        private int _notesLength;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new UploadCustomerFileViewModelValidator();
            _notesLength = typeof(Diagram)
                .GetPropertyAttribute<StringLengthAttribute>("DiagramNotes")
                .MaximumLength;
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_FileHasNoContent_ReturnsNoFileContentError()
        {
            #region Arrange

            var emptyTestFile = new Mock<HttpPostedFileBase>();
            emptyTestFile.Setup(x => x.ContentLength).Returns(0);
            emptyTestFile.Setup(x => x.ContentType).Returns(@"application/msword");


            var model = new UploadCustomerFileViewModel
            {
                CustomerFile = emptyTestFile.Object
            };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.NoFileContent, result.Errors[0].ErrorMessage);

            #endregion
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_FileHasInvalidContentType_ReturnsInvalidContentTypeError()
        {
            #region Arrange

            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/vnd.openxmlformats-officedocument.presentationml.presentation");

            var model = new UploadCustomerFileViewModel
            {
                CustomerFile = testFile.Object
            };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.InvalidContentType, result.Errors[0].ErrorMessage);

            #endregion
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_WordFileHasContentAndValidContentType_IsValid()
        {
            #region Arrange

            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/msword");

            var model = new UploadCustomerFileViewModel
            {
                EditLevel = "Level One",
                CustomerFile = testFile.Object
            };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsTrue(result.IsValid);

            #endregion
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_ExcelFileHasContentAndValidContentType_IsValid()
        {
            #region Arrange

            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/msexcel");

            var model = new UploadCustomerFileViewModel
            {
                EditLevel = "Level One",
                CustomerFile = testFile.Object
            };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsTrue(result.IsValid);

            #endregion
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_PdfFileHasContentAndValidContentType_IsValid()
        {
            #region Arrange

            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/pdf");

            var model = new UploadCustomerFileViewModel
            {
                EditLevel = "Level One",
                CustomerFile = testFile.Object
            };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsTrue(result.IsValid);

            #endregion
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_VisioFileHasContentAndValidContentType_IsValid()
        {
            #region Arrange

            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/octet-stream");

            var model = new UploadCustomerFileViewModel
            {
                EditLevel = "Level One",
                CustomerFile = testFile.Object
            };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsTrue(result.IsValid);

            #endregion
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_PackNotesIsTooLong_IsValidationError()
        {
            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/octet-stream");

            _validator.ShouldHaveValidationErrorFor(model => model.Notes,
                new UploadCustomerFileViewModel
                {
                    EditLevel = "Level One",
                    CustomerFile = testFile.Object,
                    Notes = new string('X', _notesLength + 1)
                });
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_PackNotesIsOk_NoValidationError()
        {
            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/octet-stream");

            _validator.ShouldNotHaveValidationErrorFor(model => model.Notes,
                new UploadCustomerFileViewModel
                {
                    EditLevel = "Level One",
                    CustomerFile = testFile.Object,
                    Notes = new string('X', _notesLength)
                });
        }

        [TestMethod]
        public void UploadCustomerFileViewModelValidator_Validate_PackNotesCanBeEmpty_NoValidationError()
        {
            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/octet-stream");

            _validator.ShouldNotHaveValidationErrorFor(model => model.Notes,
                new UploadCustomerFileViewModel
                {
                    EditLevel = "Level One",
                    CustomerFile = testFile.Object,
                    Notes = null
                });
        }
    }
}