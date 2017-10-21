using Fujitsu.SLM.Web.Context.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
using System.Web;

namespace Fujitsu.SLM.Web.Context.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ContextManagerTests
    {
        private Mock<HttpContextBase> _contextMock;
        private IContextManager _target;
        private Mock<IPrincipal> _principalMock;
        private Mock<IIdentity> _identityMock;

        private const string UserAccount = @"EUROPE\JordanM";


        [TestInitialize]
        public void Setup()
        {
            _principalMock = new Mock<IPrincipal>();
            _identityMock = new Mock<IIdentity>();
            _identityMock.Setup(s => s.Name).Returns(UserAccount);
            _principalMock.Setup(s => s.Identity).Returns(_identityMock.Object);

            _contextMock = new Mock<HttpContextBase>();

            _contextMock.Setup(s => s.User).Returns(_principalMock.Object);
            _target = ContextManager.GetContextManager(_contextMock.Object);
        }

        [TestMethod]
        public void ContextManager_Constructor_ReturnsInstance()
        {
            Assert.IsNotNull(_target);
        }
    }
}
