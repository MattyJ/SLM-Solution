using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Transformers.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.SLM.Transformers.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TransformSpreadsheetToTemplateTests
    {
        private Mock<IUserIdentity> _mockUserIdentity;

        private ITransformSpreadsheetToTemplate _transformSpreadsheetToTemplate;

        [TestInitialize]
        public void TestInitilize()
        {
            _mockUserIdentity = new Mock<IUserIdentity>();

            _transformSpreadsheetToTemplate = new TransformSpreadsheetToTemplate(
                _mockUserIdentity.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TransformSpreadsheetToTemplat_Contructor_NoUserIdentity_ThrowsException()
        {
            #region Arrange

            _transformSpreadsheetToTemplate = new TransformSpreadsheetToTemplate(null);

            #endregion
        }

        #endregion

    }
}
