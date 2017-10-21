using Fujitsu.SLM.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Web;

namespace Fujitsu.SLM.Web.Context.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class UserManagerTests
    {
        private const string UserAccount = @"EUROPE\JordanM";
        private Mock<HttpRequestBase> _requestMock;
        private Mock<IPrincipal> _principalMock;
        private Mock<IIdentity> _identityMock;
        private UserManager _target;

        [TestInitialize]
        public void Setup()
        {
            _requestMock = new Mock<HttpRequestBase>();
            _principalMock = new Mock<IPrincipal>();
            _identityMock = new Mock<IIdentity>();
            _identityMock.Setup(s => s.Name).Returns(UserAccount);
            _principalMock.Setup(s => s.Identity).Returns(_identityMock.Object);
            _principalMock.Setup(s => s.IsInRole(It.Is<string>(m => m == UserRoles.Administrator))).Returns(false);
            _principalMock.Setup(s => s.IsInRole(It.Is<string>(m => m == UserRoles.Architect))).Returns(true);
            _principalMock.Setup(s => s.IsInRole(It.Is<string>(m => m == UserRoles.Viewer))).Returns(true);
            _target = new UserManager(_principalMock.Object, _requestMock.Object);
        }

        [TestMethod]
        public void UserManager_Constructor_ReturnsInstance()
        {
            Assert.IsNotNull(_target);
        }

        [TestMethod]
        public void UserManager_RequestUser_CallsProperty()
        {
            var userName = _target.User.Identity.Name;
            Assert.AreEqual(UserAccount, userName);
        }

        [TestMethod]
        public void UserManager_IsRole_CallsMethod()
        {
            var role = _target.IsRole();
            Assert.AreEqual(UserRoles.Architect, role);
        }

        [TestMethod]
        public void UserManager_HasSLMAdministrator_CallsMethod()
        {
            var role = _target.HasSLMAdministrator();
            Assert.IsFalse(role);
        }

        [TestMethod]
        public void UserManager_HasSLMArchitect_CallsMethod()
        {
            var role = _target.HasSLMArchitect();
            Assert.IsTrue(role);
        }

        [TestMethod]
        public void UserManager_HasSLMViewer_CallsMethod()
        {
            var role = _target.HasSLMViewer();
            Assert.IsTrue(role);
        }

        [TestMethod]
        public void UserManager_IsSLMAdministrator_CallsMethod()
        {
            var role = _target.IsSLMAdministrator();
            Assert.IsFalse(role);
        }

        [TestMethod]
        public void UserManager_IsSLMArchitect_CallsMethod()
        {
            var role = _target.IsSLMArchitect();
            Assert.IsTrue(role);
        }

        [TestMethod]
        public void UserManager_IsSLMViewer_CallsMethod()
        {
            var role = _target.IsSLMViewer();
            Assert.IsFalse(role);
        }

        [TestMethod]
        public void UserManager_IsAuthenticated_ReturnsTrue()
        {
            _identityMock.Setup(s => s.IsAuthenticated).Returns(true);
            var authenticated = _target.IsAuthenticated();
            Assert.IsTrue(authenticated);
        }

        [TestMethod]
        public void UserManager_IsAuthenticated_ReturnsFalse()
        {
            _identityMock.Setup(s => s.IsAuthenticated).Returns(false);
            var authenticated = _target.IsAuthenticated();
            Assert.IsFalse(authenticated);
        }
    }
}
