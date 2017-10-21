using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Web.Context.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ApplicationManagerTests
    {
        private Mock<HttpApplicationStateBase> _applicationMock;
        private ApplicationManager _applicationManager;

        [TestInitialize]
        public void Setup()
        {
            this._applicationMock = new Mock<HttpApplicationStateBase>();
            this._applicationManager = new ApplicationManager(this._applicationMock.Object);
        }

        [TestMethod]
        public void ApplicationManager_Constructor_ReturnsInstance()
        {
            Assert.IsNotNull(this._applicationManager);
        }

        [TestMethod]
        public void ApplicationManager_AddToApplicationCache_VerifyUnderlyingClassIsInvoked()
        {
            this._applicationManager.Add("xkey", "xvalue");
            this._applicationMock.Verify(v => v.Add(It.Is<string>(m => m == "xkey"),
                It.Is<string>(m => m == "xvalue")),
                Times.Once());
        }

        [TestMethod]
        public void ApplicationManager_GetFromApplicationCache_VerifyUnderlyingClassIsInvoked()
        {
            this._applicationManager.Get("xkey");
            this._applicationMock.Verify(v => v.Get(It.Is<string>(m => m == "xkey")), Times.Once());
        }
    }
}
