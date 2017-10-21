using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Web.Context.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RequestManagerTests
    {
        private const string UriUrl = "http://pol.com/";
        private const string CollectionKey = "xkey";
        private const string CollectionValue = "xvalue";
        private const string VirtualPath = "~/slm/home/";
        private readonly HttpCookieCollection _cookieCollection = new HttpCookieCollection()
        {
            new HttpCookie(CollectionKey, CollectionValue)
        };
        private readonly NameValueCollection _nameValues = new NameValueCollection() { { CollectionKey, CollectionValue } };
        private Mock<HttpRequestBase> _requestMock;
        private RequestManager _target;

        [TestInitialize]
        public void Setup()
        {
            this._requestMock = new Mock<HttpRequestBase>();
            this._requestMock.Setup(r => r.Url).Returns(new Uri(UriUrl));
            this._requestMock.Setup(r => r.ApplicationPath).Returns(VirtualPath);
            this._requestMock.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns(VirtualPath);
            this._requestMock.Setup(r => r.Cookies).Returns(this._cookieCollection);
            this._requestMock.Setup(r => r.FilePath).Returns(VirtualPath);
            this._requestMock.Setup(r => r.IsAuthenticated).Returns(true);
            this._requestMock.Setup(r => r.Form).Returns(this._nameValues);
            this._requestMock.Setup(r => r.ServerVariables).Returns(this._nameValues);
            this._requestMock.Setup(r => r.QueryString).Returns(this._nameValues);
            this._requestMock.Setup(r => r.IsLocal).Returns(true);
            this._requestMock.Setup(r => r[It.Is<string>(s => s == CollectionKey)]).Returns(CollectionKey);
            this._requestMock.Setup(r => r.MapPath(It.IsAny<string>())).Returns(VirtualPath);
            this._requestMock.Setup(r => r.MapPath(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(VirtualPath);
            this._target = new RequestManager(this._requestMock.Object);
        }

        [TestMethod]
        public void RequestManager_Constructor_ReturnsInstance()
        {
            Assert.IsNotNull(this._target);
        }

        [TestMethod]
        public void RequestManager_RequestUrl_ReturnsCorrectlyFormedUri()
        {
            var uri = this._target.Url;
            Assert.IsNotNull(uri);
            Assert.AreSame(uri.AbsoluteUri, UriUrl);
            this._requestMock.Verify(r => r.Url, Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestApplicationPath_ReturnsCorrectPath()
        {
            var applicationPath = this._target.ApplicationPath;
            Assert.IsNotNull(applicationPath);
            Assert.AreSame(applicationPath, VirtualPath);
            this._requestMock.Verify(r => r.ApplicationPath, Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestAppRelativePath_ReturnsCorrectPath()
        {
            var appRelativeCurrentExecutionFilePath = this._target.AppRelativeCurrentExecutionFilePath;
            Assert.IsNotNull(appRelativeCurrentExecutionFilePath);
            Assert.AreSame(appRelativeCurrentExecutionFilePath, VirtualPath);
            this._requestMock.Verify(r => r.AppRelativeCurrentExecutionFilePath, Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestCookies_ReturnsCorrectCollection()
        {
            var cookies = this._target.Cookies;
            Assert.IsNotNull(cookies);
            Assert.AreSame(cookies[CollectionKey].Value, CollectionValue);
            this._requestMock.Verify(r => r.Cookies, Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestFilePath_ReturnsCorrectPath()
        {
            var filePath = this._target.FilePath;
            Assert.IsNotNull(filePath);
            Assert.AreSame(filePath, VirtualPath);
            this._requestMock.Verify(r => r.FilePath, Times.Once());
        }

        [TestMethod]
        public void RequestManager_CheckIfAuthentication_ReturnsCorrectValue()
        {
            var isAuthenticated = this._target.IsAuthenticated;
            Assert.IsTrue(isAuthenticated);
            this._requestMock.Verify(r => r.IsAuthenticated, Times.Once());
        }

        [TestMethod]
        public void RequestManager_CheckIfLocal_ReturnsCorrectValue()
        {
            var isLocal = this._target.IsLocal;
            Assert.IsTrue(isLocal);
            this._requestMock.Verify(r => r.IsLocal, Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestParam_ReturnsCorrectValue()
        {
            var s = this._target[CollectionKey];
            Assert.IsNotNull(s);
            Assert.AreSame(s, CollectionKey);
            this._requestMock.Verify(r => r[It.IsAny<string>()], Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestMapPath_ReturnsCorrectValue()
        {
            var s = this._target.MapPath(VirtualPath);
            Assert.IsNotNull(s);
            Assert.AreSame(s, VirtualPath);
            this._requestMock.Verify(r => r.MapPath(It.IsAny<string>()), Times.Once());
            this._requestMock.Verify(r => r.MapPath(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
        }

        [TestMethod]
        public void RequestManager_RequestMapPathAll_ReturnsCorrectValue()
        {
            var s = this._target.MapPath(VirtualPath, VirtualPath, true);
            Assert.IsNotNull(s);
            Assert.AreSame(s, VirtualPath);
            this._requestMock.Verify(r => r.MapPath(It.IsAny<string>()), Times.Never());
            this._requestMock.Verify(r => r.MapPath(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestQueryString_ReturnsCorrectValue()
        {
            var nv = this._target.QueryString;
            Assert.IsNotNull(nv);
            Assert.AreSame(nv[CollectionKey], CollectionValue);
            this._requestMock.Verify(r => r.QueryString, Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestFormValues_ReturnsCorrectValue()
        {
            var nv = this._target.Form;
            Assert.IsNotNull(nv);
            Assert.AreSame(nv[CollectionKey], CollectionValue);
            this._requestMock.Verify(r => r.Form, Times.Once());
        }

        [TestMethod]
        public void RequestManager_RequestServerVariables_ReturnsCorrectValue()
        {
            var nv = this._target.ServerVariables;
            Assert.IsNotNull(nv);
            Assert.AreSame(nv[CollectionKey], CollectionValue);
            this._requestMock.Verify(r => r.ServerVariables, Times.Once());
        }
    }
}
