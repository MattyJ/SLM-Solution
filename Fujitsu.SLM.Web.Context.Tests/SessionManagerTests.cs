using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Collections.Generic;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Web.Context.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class SessionManagerTests
    {
        private Mock<HttpSessionStateBase> _sessionMock;
        private SessionManager _target;

        [TestInitialize]
        public void Setup()
        {
            this._sessionMock = new Mock<HttpSessionStateBase>();
            this._sessionMock.Setup(s => s["GetObject"]).Returns(2 as object);
            this._sessionMock.Setup(s => s["GetGeneric"]).Returns(new List<string>());
            this._target = new SessionManager(this._sessionMock.Object);
        }

        [TestMethod]
        public void SessionManager_Constructor_ReturnsInstance()
        {
            Assert.IsNotNull(this._target);
        }

        [TestMethod]
        public void SessionManager_AddToSession_VerifyUnderlyingClassIsInvoked()
        {
            this._target.Add("Add", 2);
            this._sessionMock.Verify(s => s.Add(It.Is<string>(key => key == "Add"), It.Is<object>(value => 2 == Convert.ToInt32(value))), Times.Once());
        }

        [TestMethod]
        public void SessionManager_RemoveSession_VerifyUnderlyingClassIsInvoked()
        {
            this._target.Remove("Remove");
            this._sessionMock.Verify(s => s.Remove(It.Is<string>(key => key == "Remove")), Times.Once());
        }

        [TestMethod]
        public void SessionManager_GetFromSession_VerifyUnderlyingClassIsInvoked()
        {
            var result = this._target.Get("GetObject");
            Assert.IsNotNull(result);
            Assert.AreEqual(result, 2);
        }

        [TestMethod]
        public void SessionManager_GetFromSessionGeneric_VerifyUnderlyingClassIsInvoked()
        {
            var result = this._target.Get<List<string>>("GetGeneric");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SessionManager_RequestAbandon_VerifyUnderlyingClassIsInvoked()
        {
            this._target.Abandon();
            this._sessionMock.Verify(s => s.Abandon(), Times.Once());
        }

        [TestMethod]
        public void SessionManager_RequestClear_VerifyUnderlyingClassIsInvoked()
        {
            this._target.Clear();
            this._sessionMock.Verify(s => s.Clear(), Times.Once());
        }
    }
}
