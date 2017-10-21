using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Controllers;
using Fujitsu.SLM.Web.Interfaces;
using Microsoft.AspNet.Identity;
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
    public class MenuControllerTests
    {
        private Mock<IAppUserContext> _mockAppUserContext;
        private Mock<IContextManager> _mockContextManager;
        private Mock<IUserManager> _mockUserManager;
        private Mock<ICacheManager> _mockCacheManager;
        private MenuController _controller;
        private AppContext _appContext;
        private Func<string> _cacheManagerFunc;
        private const string HelpUrl = "http://bbc.co.uk";
        private Mock<ApplicationUserManager> _mockApplicationUserManager;
        private Mock<IUserStore<ApplicationUser>> _mockUserStore;

        [TestInitialize]
        public void Initialize()
        {
            _appContext = new AppContext();
            _mockAppUserContext = new Mock<IAppUserContext>();
            _mockAppUserContext.Setup(s => s.Current).Returns(_appContext);

            _mockUserManager = new Mock<IUserManager>();
            _mockUserManager.Setup(s => s.HasSLMAdministrator()).Returns(true);
            _mockUserManager.Setup(s => s.HasSLMArchitect()).Returns(true);
            _mockUserManager.Setup(s => s.HasSLMViewer()).Returns(true);

            _mockContextManager = new Mock<IContextManager>();
            _mockContextManager.Setup(s => s.UserManager).Returns(_mockUserManager.Object);

            _mockCacheManager = new Mock<ICacheManager>();
            _mockCacheManager.Setup(s => s.ExecuteAndCache(It.IsAny<string>(), It.IsAny<Func<string>>()))
                .Returns(HelpUrl)
                .Callback<string, Func<string>>((s, func) => _cacheManagerFunc = func);

            _mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            _mockApplicationUserManager = new Mock<ApplicationUserManager>(_mockUserStore.Object);

            _controller = new MenuController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockCacheManager.Object,
                _mockApplicationUserManager.Object);
        }

        #region Constructor Tests

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MenuController_Constructor_ContextManagerNull_ThrowsArgumentNullException()
        {
            new MenuController(null,
                _mockAppUserContext.Object,
                _mockCacheManager.Object,
                _mockApplicationUserManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MenuController_Constructor_AppUserContextNull_ThrowsArgumentNullException()
        {
            new MenuController(_mockContextManager.Object,
                null,
                _mockCacheManager.Object,
                _mockApplicationUserManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MenuController_Constructor_CacheManagerNull_ThrowsArgumentNullException()
        {
            new MenuController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                null,
                _mockApplicationUserManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MenuController_Constructor_ApplicationUserManagerNull_ThrowsArgumentNullException()
        {
            new MenuController(_mockContextManager.Object,
                _mockAppUserContext.Object,
                _mockCacheManager.Object,
                null);
        }

        #endregion



        [TestMethod]
        public void MenuController_HomeMenu_ReturnsView_IsPartialView()
        {
            var result = _controller.HomeMenu();
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void MenuController_HomeMenu_ReturnsView_IsHomeMenuView()
        {
            var result = _controller.HomeMenu() as PartialViewResult;
            Assert.AreEqual("_HomeMenu", result.ViewName);
        }

        [TestMethod]
        public void MenuController_QuickLinks_ReturnsView_IsPartialView()
        {
            var result = _controller.QuickLinks();
            Assert.IsInstanceOfType(result, typeof(PartialViewResult));
        }

        [TestMethod]
        public void MenuController_QuickLinks_ReturnsView_IsQuickLinksView()
        {
            var result = _controller.QuickLinks() as PartialViewResult;
            Assert.AreEqual("_QuickLinks", result.ViewName);
        }

        [TestMethod]
        public void MenuController_QuickLinks_Attributes_NoHttpGet()
        {
            Assert.IsNull(_controller.GetMethodAttribute<HttpGetAttribute>("QuickLinks"));
        }

        [TestMethod]
        public void MenuController_QuickLinks_Attributes_NoHttpPost()
        {
            Assert.IsNull(_controller.GetMethodAttribute<HttpPostAttribute>("QuickLinks"));
        }

        [TestMethod]
        public void MenuController_HomeMenu_Attributes_NoHttpGet()
        {
            Assert.IsNull(_controller.GetMethodAttribute<HttpGetAttribute>("HomeMenu"));
        }

        [TestMethod]
        public void MenuController_HomeMenu_Attributes_NoHttpPost()
        {
            Assert.IsNull(_controller.GetMethodAttribute<HttpPostAttribute>("HomeMenu"));
        }
    }
}
