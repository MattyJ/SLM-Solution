using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Fujitsu.SLM.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Web.Tests
{
    /// <summary>
    /// Summary description for ErrorControllerTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ErrorControllerTests
    {
        private ErrorController _errorController;

        #region Test Initialize

        [TestInitialize]
        public void Initialize()
        {
            _errorController = new ErrorController();
        }

        #endregion

        #region General Error

        [TestMethod]
        public void ErrorController_GeneralError_ReturnsView()
        {
            #region Arrange

            var ex = new ApplicationException();

            #endregion

            #region Act

            var result = _errorController.GeneralError(ex);

            #endregion

            #region Assert

            Assert.IsNotNull(result);

            var view = result as ViewResult;
            Assert.IsNotNull(view);
            Assert.IsInstanceOfType(view.Model, typeof(ApplicationException));

            #endregion
        }

        #endregion

        #region General Error

        [TestMethod]
        public void ErrorController_SessionExpired_ReturnsView()
        {
            #region Arrange

            #endregion

            #region Act

            var result = _errorController.SessionExpired();

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            var view = result as ViewResult;
            Assert.IsNotNull(view);

            #endregion
        }

        #endregion

        #region General Error

        [TestMethod]
        public void ErrorController_Unauthorized_ReturnsView()
        {
            #region Arrange

            var ex = new ApplicationException();
            #endregion

            #region Act


            var result = _errorController.Unauthorized();

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            var view = result as ViewResult;
            Assert.IsNotNull(view);

            #endregion
        }

        [TestMethod]
        public void ErrorController_NoRoles_ReturnsView()
        {
            #region Arrange

            var ex = new ApplicationException();
            #endregion

            #region Act


            var result = _errorController.NoRoles("test@test.com");

            #endregion

            #region Assert

            Assert.IsNotNull(result);
            var view = result as ViewResult;
            Assert.IsNotNull(view);

            #endregion
        }


        #endregion

    }
}
