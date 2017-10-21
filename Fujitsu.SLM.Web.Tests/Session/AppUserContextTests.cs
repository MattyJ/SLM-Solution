using System;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models.Session;
using Fujitsu.SLM.Web.Session;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Tests.Session
{
    [TestClass]
    public class AppUserContextTests
    {
        private Mock<IContextManager> _mockContextManager;
        private Mock<ISessionManager> _mockSessionManager;

        private AppContext _appContext;
        private IAppUserContext _target;

        [TestInitialize]
        public void TestInitialize()
        {
            _appContext = new AppContext();
            _mockContextManager = new Mock<IContextManager>();

            _mockSessionManager = new Mock<ISessionManager>();
            _mockSessionManager
                .Setup(s => s.Get<AppContext>(SessionNames.AppContext))
                .Returns(_appContext);

            _mockContextManager.Setup(s => s.SessionManager).Returns(_mockSessionManager.Object);

            _target = new AppUserContext(_mockContextManager.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AppUserContext_Constructor_ContextManagerIsNull_ThrowsArgumentNullException()
        {
            new AppUserContext(null);
        }

        [TestMethod]
        public void AppUserContext_Constructor_NewAppContextCreated()
        {
            _mockSessionManager.Setup(s => s.Get<AppContext>(SessionNames.AppContext)).Returns(null as AppContext);
            _target = new AppUserContext(_mockContextManager.Object);
            var result = _target.Current;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void AppUserContext_Constructor_SessionManagerDoesNotHoldAppContext_NewAppContextCreated()
        {
            _mockSessionManager.Setup(s => s.Get<AppContext>(SessionNames.AppContext)).Returns(null as AppContext);
            _target = new AppUserContext(_mockContextManager.Object);
            var result = _target.Current;
            Assert.IsNotNull(result);
        }
    }
}
