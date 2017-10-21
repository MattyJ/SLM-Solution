using System;
using System.Collections.Generic;
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
    public class ResponseManagerTests
    {
        private const string CollectionKey = "xkey";
        private const string CollectionValue = "xvalue";
        private readonly HttpCookieCollection _cookieCollection = new HttpCookieCollection()
        {
            new HttpCookie(CollectionKey, CollectionValue)
        };

        private Mock<HttpResponseBase> _requestMock;
        private ResponseManager _target;

        [TestInitialize]
        public void Setup()
        {
            this._requestMock = new Mock<HttpResponseBase>();
            this._requestMock.Setup(r => r.Cookies).Returns(_cookieCollection);
            this._target = new ResponseManager(this._requestMock.Object);
        }

        [TestMethod]
        public void ResponseManager_Constructor_ReturnsInstance()
        {
            Assert.IsNotNull(this._target);
        }

        [TestMethod]
        public void ResponseManager_RequestAppRelativePath_ReturnsCorrectPath()
        {
            var cookies = this._target.Cookies;
            Assert.IsNotNull(cookies);
            this._requestMock.Verify(r => r.Cookies, Times.Once());
        }

        [TestMethod]
        public void ResponseManager_RequestCookies_ReturnsCorrectCollection()
        {
            this._target.Cookies.Add(new HttpCookie("CookieMonster", "Yes"));
            this._requestMock.Verify(r => r.Cookies, Times.Once());
            Assert.IsNotNull(this._cookieCollection["CookieMonster"]);
        }

        [TestMethod]
        public void ResponseManager_StatusCode_Set_ValueIsSet()
        {
            this._target.StatusCode = 500;
            this._requestMock.VerifySet(r => r.StatusCode = 500, Times.Once());
        }

        [TestMethod]
        public void ResponseManager_StatusCode_Get_ValueIsGet()
        {
            var code = this._target.StatusCode;
            this._requestMock.VerifyGet(r => r.StatusCode, Times.Once());
        }

        [TestMethod]
        public void ResponseManager_AppendHeader_ValueIsSet()
        {
            this._target.AppendHeader("XXX", "YYY");
            this._requestMock.Verify(v => v.AppendHeader("XXX", "YYY"));
        }
    }
}