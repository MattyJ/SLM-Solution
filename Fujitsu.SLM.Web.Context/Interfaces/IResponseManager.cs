using System.Web;

namespace Fujitsu.SLM.Web.Context.Interfaces
{
    public interface IResponseManager
    {
        /// <summary>
        /// Gets the response cookie collection.
        /// </summary>
        HttpCookieCollection Cookies { get; }

        /// <summary>
        /// The status code of the HTTP output that is returned to the client. For information
        /// about valid status codes, see HTTP Status Codes on the MSDN Web site.
        /// </summary>
        int StatusCode { get; set; }

        /// <summary>
        /// Adds an HTTP header to the output stream.
        /// </summary>
        /// <param name="name">The name of the HTTP header to add to the output stream.</param>
        /// <param name="value">The string to append to the header.</param>
        void AppendHeader(string name, string value);
    }
}