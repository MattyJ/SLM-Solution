using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Models;
using Fujitsu.SLM.Web.Resources;
using Fujitsu.SLM.Web.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace Fujitsu.SLM.Web.Tests.Validators
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ImportTemplateViewModelValidatorTests
    {
        private ImportTemplateViewModelValidator _validator;

        [TestInitialize]
        public void Initialize()
        {
            _validator = new ImportTemplateViewModelValidator();
        }

        [TestMethod]
        public void ImportTemplateViewModelValidator_Validate_NoTemplateType_ReturnsNoTemplateTypeSelectedError()
        {
            #region Arrange

            var model = new ImportTemplateViewModel();

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.NoTemplateTypeSelected, result.Errors[0].ErrorMessage);

            #endregion
        }

        [TestMethod]
        public void ImportTemplateViewModelValidator_Validate_NoFileSelected_ReturnsNoFileSelectedError()
        {
            #region Arrange

            var model = new ImportTemplateViewModel { TemplateType = TemplateTypeNames.SORT };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual(WebResources.NoFileSelected, result.Errors[0].ErrorMessage);

            #endregion
        }

        [TestMethod]
        public void ImportTemplateViewModelValidator_Validate_FileHasNoContent_ReturnsNoFileContentError()
        {
            #region Arrange

            var emptyTestFile = new Mock<HttpPostedFileBase>();

            var model = new ImportTemplateViewModel
            {
                TemplateType = TemplateTypeNames.SORT,
                SpreadsheetFile = emptyTestFile.Object
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
        public void ImportTemplateViewModelValidator_Validate_FileHasInvalidContentType_ReturnsInvalidContentTypeError()
        {
            #region Arrange

            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/msword");

            var model = new ImportTemplateViewModel
            {
                TemplateType = TemplateTypeNames.SLM,
                SpreadsheetFile = testFile.Object
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
        public void ImportTemplateViewModelValidator_Validate_FileHasContentAndValidContentType_IsValid()
        {
            #region Arrange

            var testFile = new Mock<HttpPostedFileBase>();
            testFile.Setup(x => x.ContentLength).Returns(30);
            testFile.Setup(x => x.ContentType).Returns(@"application/msexcel");

            var model = new ImportTemplateViewModel
            {
                TemplateType = TemplateTypeNames.SORT,
                SpreadsheetFile = testFile.Object
            };

            #endregion

            #region Act

            var result = _validator.Validate(model);

            #endregion

            #region Assert

            Assert.IsTrue(result.IsValid);

            #endregion
        }
    }
}
