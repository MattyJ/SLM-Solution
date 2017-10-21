using System.Web;
using Fujitsu.SLM.Web.Context.Interfaces;

namespace Fujitsu.SLM.Web.Context
{
    /// <summary>
    /// A class used to wrap the HttpContext Response object.
    /// </summary>
    public class ResponseManager : IResponseManager
    {
        private readonly HttpResponseBase _response;

        public int StatusCode
        {
            get { return this._response.StatusCode; }
            set { this._response.StatusCode = value; }
        }

        public HttpCookieCollection Cookies
        {
            get { return this._response.Cookies; }
        }

        public ResponseManager(HttpResponseBase response)
        {
            this._response = response;
        }

        public void AppendHeader(string name, string value)
        {
            this._response.AppendHeader(name, value);
        }
    }
}