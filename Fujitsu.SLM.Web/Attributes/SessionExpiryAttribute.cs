using System;
using System.Web.Mvc;
using Fujitsu.SLM.Constants;

namespace Fujitsu.SLM.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SessionExpiryAttribute : ActionFilterAttribute
    {
        // Force a redirect to a warning page if there is a session timeout.
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Don't check for session timeout if we are currently loading an error page
            if (filterContext.HttpContext.Request.Url.AbsolutePath.IndexOf("/Error/", StringComparison.OrdinalIgnoreCase) < 0)
            {
                if ((filterContext.HttpContext.Session != null) && filterContext.HttpContext.Session.IsNewSession)
                {
                    // A new session has just been started. If there's still a cookie left from a previous session, there must have been a timeout
                    var cookie = filterContext.HttpContext.Request.Headers["Cookie"];
                    if ((cookie != null) && (cookie.IndexOf("_SessionId", StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        if (filterContext.HttpContext.Request.IsAjaxRequest())
                        {
                            // AJAX Request. Send back a
                            filterContext.HttpContext.Response.StatusCode = 500;
                            filterContext.HttpContext.Response.AppendHeader(ModelStateErrorNames.ErrorMessage, ModelStateErrorNames.SessionExpired);
                            return;
                        }

                        // HTTP Request. Redirect to an error page
                        filterContext.Result = new RedirectResult("~/Error/SessionExpired");
                        return;
                    }
                }
            }

            base.OnActionExecuting(filterContext);
        }

    }
}
