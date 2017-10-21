using Fujitsu.SLM.Web.Context.Interfaces;
using System.Web;

namespace Fujitsu.SLM.Web.Context
{
    public class ContextManager : IContextManager
    {
        private readonly HttpContextBase _context;

        public ContextManager()
            : this(new HttpContextWrapper(HttpContext.Current))
        {
        }

        private ContextManager(HttpContextBase context)
        {
            _context = context;
            SessionManager = new SessionManager(_context.Session);
            RequestManager = new RequestManager(_context.Request);
            ResponseManager = new ResponseManager(_context.Response);
            ApplicationManager = new ApplicationManager(_context.Application);

            UserManager = !_context.User.Identity.IsAuthenticated
                ? (IUserManager)new NullUserManager(null, _context.Request)
                : new UserManager(_context.User, _context.Request);
        }

        /// <summary>
        /// Get the SessionManager for the current context.
        /// </summary>
        public ISessionManager SessionManager { get; private set; }

        /// <summary>
        /// Get the RequestManager for the current context.
        /// </summary>
        public IRequestManager RequestManager { get; private set; }

        /// <summary>
        /// Get the ResponseManager for the current context.
        /// </summary>
        public IResponseManager ResponseManager { get; private set; }

        /// <summary>
        /// Get the ApplicationManager for the current context.
        /// </summary>
        public IApplicationManager ApplicationManager { get; private set; }

        /// <summary>
        /// Gets or sets UserManager for the current context.
        /// </summary>
        public IUserManager UserManager { get; private set; }

        public static IContextManager GetContextManager(HttpContextBase context)
        {
            return new ContextManager(context);
        }

        /// <summary>
        /// Gets an application level resource.
        /// </summary>
        /// <param name="classKey"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        public object GetGlobalResourceObject(string classKey, string resourceKey)
        {
            return _context.GetGlobalResourceObject(classKey, resourceKey);
        }

        /// <summary>
        /// Gets an local level resource.
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        public object GetLocalResourceObject(string virtualPath, string resourceKey)
        {
            return _context.GetLocalResourceObject(virtualPath, resourceKey);
        }
    }
}
