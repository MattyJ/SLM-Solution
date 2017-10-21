using System.Web;
using Moq;

namespace Fujitsu.SLM.UnitTesting
{
    public class ControllerContextMocks
    {
        public Mock<HttpContextBase> MockHttpContextBase { get; set; }
        public Mock<HttpRequestBase> MockHttpRequestBase { get; set; }
        public Mock<HttpResponseBase> MockHttpResponseBase { get; set; }

        public ControllerContextMocks()
        {
            this.MockHttpRequestBase = new Mock<HttpRequestBase>();
            this.MockHttpRequestBase.Setup(r => r.AppRelativeCurrentExecutionFilePath).Returns("/");
            this.MockHttpRequestBase.Setup(r => r.ApplicationPath).Returns("/");
            this.MockHttpResponseBase = new Mock<HttpResponseBase>();
            this.MockHttpResponseBase.Setup(s => s.ApplyAppPathModifier(It.IsAny<string>())).Returns<string>(s => s);
            this.MockHttpContextBase = new Mock<HttpContextBase>();
            this.MockHttpContextBase.Setup(h => h.Request).Returns(this.MockHttpRequestBase.Object);
            this.MockHttpContextBase.Setup(h => h.Response).Returns(this.MockHttpResponseBase.Object);
        }
    }
}