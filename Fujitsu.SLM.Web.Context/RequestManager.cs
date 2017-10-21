using System;
using System.Collections.Specialized;
using System.Web;
using Fujitsu.SLM.Web.Context.Interfaces;

namespace Fujitsu.SLM.Web.Context
{

    /// <summary>
    /// A class used to wrap the HttpContext Request object.
    /// </summary>
    public class RequestManager : IRequestManager
    {
        private readonly HttpRequestBase _request;

        /// <summary>
        /// Create a Request wrapper.
        /// </summary>
        /// <param name="request"></param>
        public RequestManager(HttpRequestBase request)
        {
            this._request = request;
        }

        /// <summary>
        /// Get the Url of this Request.
        /// </summary>
        public Uri Url
        {
            get
            {
                return this._request.Url;
            }
        }

        /// <summary>
        /// Gets the virtual root of the application.
        /// </summary>
        public string ApplicationPath
        {
            get
            {
                return this._request.ApplicationPath;
            }
        }

        /// <summary>
        /// Gets the virtual root of the application and makes it relative.
        /// </summary>
        public string AppRelativeCurrentExecutionFilePath
        {
            get
            {
                return this._request.AppRelativeCurrentExecutionFilePath;
            }
        }

        /// <summary>
        /// Gets the virtual root of the application and makes it relative.
        /// </summary>
        public HttpCookieCollection Cookies
        {
            get
            {
                return this._request.Cookies;
            }
        }

        /// <summary>
        /// Gets the virtual path of the current request.
        /// </summary>
        public string FilePath
        {
            get
            {
                return this._request.FilePath;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the request has been authenticated.
        /// </summary>
        public bool IsAuthenticated
        {
            get
            {
                return this._request.IsAuthenticated;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether the request is from the local computer.
        /// </summary>
        public bool IsLocal
        {
            get
            {
                return this._request.IsLocal;
            }
        }

        /// <summary>
        /// Gets the collection of HTTP query-string variables.
        /// </summary>
        public NameValueCollection QueryString
        {
            get
            {
                return this._request.QueryString;
            }
        }

        /// <summary>
        /// Gets a collection of Web server variables.
        /// </summary>
        public NameValueCollection ServerVariables
        {
            get
            {
                return this._request.ServerVariables;
            }
        }

        /// <summary>
        /// Gets the collection of form variables that were sent by the client.
        /// </summary>
        public NameValueCollection Form
        {
            get
            {
                return this._request.Form;
            }
        }

        /// <summary>
        /// Get the specified object from the collections.
        /// </summary>
        /// <param name="key">The key of the item to be retrieved.</param>
        /// <returns>The object if found.</returns>
        public string this[string key]
        {
            get
            {
                return this._request[key];
            }
        }

        /// <summary>
        /// Maps the specified virtual path to a physical path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file.</param>
        /// <returns></returns>
        public string MapPath(string virtualPath)
        {
            return this._request.MapPath(virtualPath);
        }


        /// <summary>
        /// Maps the specified virtual path to a physical path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file.</param>
        /// <param name="baseVirtualDir">The virtual base path that is used for relative resolution.</param>
        /// <param name="allowCrossAppMapping">True to indicate that the virtualPath can belong to another application.</param>
        /// <returns></returns>
        public string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping)
        {
            return this._request.MapPath(virtualPath, baseVirtualDir, allowCrossAppMapping);
        }
    }
}
