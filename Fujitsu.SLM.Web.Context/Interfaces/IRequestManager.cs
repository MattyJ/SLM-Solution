using System;
using System.Collections.Specialized;
using System.Web;

namespace Fujitsu.SLM.Web.Context.Interfaces
{
    public interface IRequestManager
    {
        /// <summary>
        /// Get the Url of this Request.
        /// </summary>
        Uri Url { get; }

        /// <summary>
        /// Gets the virtual root of the application.
        /// </summary>
        string ApplicationPath { get; }

        /// <summary>
        /// Gets the virtual root of the application and makes it relative.
        /// </summary>
        string AppRelativeCurrentExecutionFilePath { get; }

        /// <summary>
        /// Gets the virtual root of the application and makes it relative.
        /// </summary>
        HttpCookieCollection Cookies { get; }

        /// <summary>
        /// Gets the virtual path of the current request.
        /// </summary>
        string FilePath { get; }

        /// <summary>
        /// Gets a value that indicates whether the request has been authenticated.
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// Gets a value that indicates whether the request is from the local computer.
        /// </summary>
        bool IsLocal { get; }

        /// <summary>
        /// Gets the collection of HTTP query-string variables.
        /// </summary>
        NameValueCollection QueryString { get; }

        /// <summary>
        /// Gets a collection of Web server variables.
        /// </summary>
        NameValueCollection ServerVariables { get; }

        /// <summary>
        /// Gets the collection of form variables that were sent by the client.
        /// </summary>
        NameValueCollection Form { get; }

        /// <summary>
        /// Get the specified object from the collections.
        /// </summary>
        /// <param name="key">The key of the item to be retrieved.</param>
        /// <returns>The object if found.</returns>
        string this[string key] { get; }

        /// <summary>
        /// Maps the specified virtual path to a physical path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file.</param>
        /// <returns></returns>
        string MapPath(string virtualPath);

        /// <summary>
        /// Maps the specified virtual path to a physical path.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the file.</param>
        /// <param name="baseVirtualDir">The virtual base path that is used for relative resolution.</param>
        /// <param name="allowCrossAppMapping">True to indicate that the virtualPath can belong to another application.</param>
        /// <returns></returns>
        string MapPath(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping);
    }
}
