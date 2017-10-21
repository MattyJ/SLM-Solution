namespace Fujitsu.SLM.Web.Context.Interfaces
{
    public interface IContextManager
    {
        /// <summary>
        /// Get the SessionManager for the current context.
        /// </summary>
        ISessionManager SessionManager { get; }

        /// <summary>
        /// Get the RequestManager for the current context.
        /// </summary>
        IRequestManager RequestManager { get; }

        /// <summary>
        /// Get the ResponseManager for the current context.
        /// </summary>
        IResponseManager ResponseManager { get; }

        /// <summary>
        /// Get the ApplicationManager for the current context.
        /// </summary>
        IApplicationManager ApplicationManager { get; }

        /// <summary>
        /// Gets or sets security information for the current HTTP request.
        /// </summary>
        IUserManager UserManager { get; }

        /// <summary>
        /// Gets an application level resource.
        /// </summary>
        /// <param name="classKey"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        object GetGlobalResourceObject(string classKey, string resourceKey);

        /// <summary>
        /// Gets an local level resource.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        object GetLocalResourceObject(string virtualPath, string resourceKey);
    }
}
