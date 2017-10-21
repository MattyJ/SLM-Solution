using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HomeControllerTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<ICacheManager> _mockCacheManager;
        private Mock<IParameterService> _mockParameterService;
        private HomeController _controller;
        private AppContext _appContext;

        [TestInitialize]
        public void Initialize()
        {
            _appContext = new AppContext();

            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockUserManager = new Mock<IUserManager>();
            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);

            _mockCacheManager = new Mock<ICacheManager>();
            _mockCacheManager.Setup(x => x.ExecuteAndCache(It.IsAny<string>(), It.IsAny<Func<string>>())).Returns("mailto:beardy@mustache-seeds.com");

            _mockParameterService = new Mock<IParameterService>();

            _controller = new HomeController(
                _mockAppUserContext.Object,
                _mockCacheManager.Object,
                _mockContextManager.Object,
                _mockUserManager.Object,
                _mockParameterService.Object
                );

            Bootstrapper.SetupAutoMapper();
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HomeController_Constructor_NoAppUserContextThrowsException()
        {
            new HomeController(
                null,
                _mockCacheManager.Object,
                _mockContextManager.Object,
                _mockUserManager.Object,
                _mockParameterService.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HomeController_Constructor_NoCacheManagerThrowsException()
        {
            new HomeController(
                _mockAppUserContext.Object,
                null,
                _mockContextManager.Object,
                _mockUserManager.Object,
                _mockParameterService.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HomeController_Constructor_NoContextManagerThrowsException()
        {
            new HomeController(
                _mockAppUserContext.Object,
                _mockCacheManager.Object,
                null,
                _mockUserManager.Object,
                _mockParameterService.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HomeController_Constructor_NoAppUserManagerThrowsException()
        {
            new HomeController(
                _mockAppUserContext.Object,
                _mockCacheManager.Object,
                _mockContextManager.Object,
                null,
                _mockParameterService.Object
                );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void HomeController_Constructor_NoParamterServiceThrowsException()
        {
            new HomeController(
                _mockAppUserContext.Object,
                _mockCacheManager.Object,
                _mockContextManager.Object,
                _mockUserManager.Object,
                null
                );
        }

        #endregion

        [TestMethod]
        public void HomeController_Index_Get_ReturnsViewResult()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Architect);
            var result = _controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HomeController_Index_Get_User_With_No_Roles_ReturnsRedirect()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.None);
            var result = _controller.Index() as RedirectToRouteResult;
            Assert.AreEqual("Error", result.RouteValues["controller"]);
            Assert.AreEqual("NoRoles", result.RouteValues["action"]);
        }

        [TestMethod]
        public void HomeController_Index_Get_ApplicationUserContextIsCleated()
        {
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            var result = _controller.Index() as ViewResult;
            var context = _mockAppUserContext.Object;

            Assert.IsNotNull(result);
            Assert.AreEqual(0, context.Current.CurrentCustomer.Id);
            Assert.AreEqual("None Selected", context.Current.CurrentCustomer.CustomerName);
        }

        [TestMethod]
        public void HomeController_About_Get_ReturnsViewResult()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Architect);
            var result = _controller.About() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HomeController_Roadmap_Get_ReturnsViewResult()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Architect);
            var result = _controller.Roadmap() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HomeController_WhatsNew_Get_ReturnsViewResult()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Architect);
            var result = _controller.WhatsNew() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void HomeController_ReleaseHistory_Get_ReturnsViewResult()
        {
            _mockUserManager.Setup(s => s.IsAuthenticated()).Returns(true);
            _mockUserManager.Setup(s => s.IsRole()).Returns(UserRoles.Architect);
            var result = _controller.ReleaseHistory() as ViewResult;
            Assert.IsNotNull(result);
        }

    }
}
