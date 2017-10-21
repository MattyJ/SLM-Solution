using System;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;
using Fujitsu.SLM.Web.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Fujitsu.SLM.Web.Tests.Attributes
{
    [TestClass]
    public class SessionExpiryAttributeTests
    {
        private SessionExpiryAttribute _target;

        private Mock<HttpSessionStateBase> _mockHttpSessionStateBase;
        private Mock<HttpRequestBase> _mockHttpRequestBase;
        private Mock<HttpResponseBase> _mockHttpResponseBase;
        private Mock<HttpContextBase> _mockHttpContextBase;
        private Mock<ActionExecutingContext> _mockActionExecutingContext;
        private HttpCookieCollection _cookies;
        private NameValueCollection _requestHeaders;
        private NameValueCollection _responseHeaders;

        private const string cookie =
            "Fujitsu.SLM={\"UserId\":\"JordanM\",\"DisplayName\":\"Matt Jordan\",\"EmailAddress\":\"matthew.jordan@uk.fujitsu.com\",\"Roles\":[\"Administrator\",\"Manager\",\"Viewer\"]}; __RequestVerificationToken=DL8OARmaALvekckGZNeQD0vQ2Fuy2OF07EH9suy2l3ZEgciImW6sTPP1Dgx215xm04YYLe19QkbCL3I0mckFT0dpOIYXU-aNoyOe_BIgDgU1; ASP.NET_SessionId=yogvsv2zmyjga12c5nx2uyq3";

        [TestInitialize]
        public void TestInitialize()
        {
            _requestHeaders = new NameValueCollection();
            _responseHeaders = new NameValueCollection();
            _cookies = new HttpCookieCollection();

            _mockHttpRequestBase = new Mock<HttpRequestBase>();
            _mockHttpRequestBase.Setup(x => x.Url).Returns(new Uri("http://www.fujitsu.com/slm/home/index"));
            _mockHttpRequestBase.Setup(x => x.Cookies).Returns(_cookies);
            _mockHttpRequestBase.Setup(x => x.Headers).Returns(_requestHeaders);

            _mockHttpResponseBase = new Mock<HttpResponseBase>();
            _mockHttpResponseBase.SetupProperty(x => x.StatusCode);
            _mockHttpResponseBase.Setup(x => x.Headers).Returns(_responseHeaders);

            _mockHttpSessionStateBase = new Mock<HttpSessionStateBase>();
            _mockHttpSessionStateBase.SetupGet(x => x.IsNewSession).Returns(false);

            _mockHttpContextBase = new Mock<HttpContextBase>();
            _mockHttpContextBase.Setup(x => x.Request).Returns(_mockHttpRequestBase.Object);
            _mockHttpContextBase.Setup(x => x.Response).Returns(_mockHttpResponseBase.Object);
            _mockHttpContextBase.Setup(x => x.Session).Returns(_mockHttpSessionStateBase.Object);

            _mockActionExecutingContext = new Mock<ActionExecutingContext>();
            _mockActionExecutingContext.Setup(x => x.HttpContext).Returns(_mockHttpContextBase.Object);

            this._target = new SessionExpiryAttribute();
        }


        [TestMethod]
        public void SessionTimeoutAttribute_OnActionExecuting_NullSession_DoesNothing()
        {
            _mockHttpContextBase.Setup(x => x.Session).Returns<HttpSessionStateBase>(null);
            _target.OnActionExecuting(_mockActionExecutingContext.Object);
        }

        [TestMethod]
        public void SessionTimeoutAttribute_OnActionExecuting_ExistingSession_DoesNothing()
        {
            _target.OnActionExecuting(_mockActionExecutingContext.Object);

            _mockHttpResponseBase.Verify(x => x.AppendHeader(It.Is<string>(s => s == "ErrorMessage"), It.Is<string>(s => s == "SessionExpired")), Times.Never);
            ActionExecutingContext actionExecutingContext = _mockActionExecutingContext.Object;
            Assert.IsNull(actionExecutingContext.Result);
        }

        [TestMethod]
        public void SessionTimeoutAttribute_OnActionExecuting_NewSessionNoCookie_DoesNothing()
        {
            _mockHttpSessionStateBase.SetupGet(x => x.IsNewSession).Returns(true);
            _target.OnActionExecuting(_mockActionExecutingContext.Object);

            _mockHttpResponseBase.Verify(x => x.AppendHeader(It.Is<string>(s => s == "ErrorMessage"), It.Is<string>(s => s == "SessionExpired")), Times.Never);
            ActionExecutingContext actionExecutingContext = _mockActionExecutingContext.Object;
            Assert.IsNull(actionExecutingContext.Result);
        }

        [TestMethod]
        public void SessionTimeoutAttribute_OnActionExecuting_NewSessionWithCookie_ReturnsRedirectResult()
        {
            _mockHttpSessionStateBase.SetupGet(x => x.IsNewSession).Returns(true);
            _requestHeaders.Add("Cookie", cookie);

            _target.OnActionExecuting(_mockActionExecutingContext.Object);

            ActionExecutingContext actionExecutingContext = _mockActionExecutingContext.Object;
            Assert.IsInstanceOfType(actionExecutingContext.Result, typeof(RedirectResult));
        }

        [TestMethod]
        public void SessionTimeoutAttribute_OnActionExecuting_NewSessionWithCookieAjax_ReturnsStatus500()
        {
            _mockHttpSessionStateBase.SetupGet(x => x.IsNewSession).Returns(true);
            _requestHeaders.Add("Cookie", cookie);
            _requestHeaders.Add("X-Requested-With", "XMLHttpRequest"); // Simulates an AJAX request

            _target.OnActionExecuting(_mockActionExecutingContext.Object);

            HttpResponseBase httpResponseBase = _mockHttpResponseBase.Object;
            Assert.AreEqual(500, httpResponseBase.StatusCode);
        }

        [TestMethod]
        public void SessionTimeoutAttribute_OnActionExecuting_NewSessionWithCookieAjax_AddsErrorMessageHeader()
        {
            _mockHttpSessionStateBase.SetupGet(x => x.IsNewSession).Returns(true);
            _requestHeaders.Add("Cookie", cookie);
            _requestHeaders.Add("X-Requested-With", "XMLHttpRequest"); // Simulates an AJAX request

            _target.OnActionExecuting(_mockActionExecutingContext.Object);

            HttpResponseBase httpResponseBase = _mockHttpResponseBase.Object;
            _mockHttpResponseBase.Verify(x => x.AppendHeader(It.Is<string>(s => s == "ErrorMessage"), It.Is<string>(s => s == "SessionExpired")), Times.Once);
        }


    }
}
