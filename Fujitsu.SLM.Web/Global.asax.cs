using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Fujitsu.SLM.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            Bootstrapper.SetupAutoMapper();

            ValidationConfig.Configure();
        }

        protected void Application_Error()
        {
            var unhandledException = Server.GetLastError();

            try
            {
                // Handle exception as per policy
                ExceptionPolicy.HandleException(unhandledException, ExceptionPolicies.General);
            }
            catch (Exception ex)
            {
                // Obtain the wrapped exception with the error handling instance id
                unhandledException = ex;
            }

            //Clear the error on the server
            Response.Clear();
            Server.ClearError();

            // Setup route to Error Controller
            var routeData = new RouteData();
            routeData.Values["controller"] = "Error";
            routeData.Values["action"] = "GeneralError";
            routeData.Values["exception"] = unhandledException;

            // Ensure content-type header is present
            Response.Headers.Add("Content-Type", "text/html");

            IController errorController = new Controllers.ErrorController();
            var wrapper = new HttpContextWrapper(Context);
            var rc = new RequestContext(wrapper, routeData);
            errorController.Execute(rc);
        }

        void Application_EndRequest(object sender, System.EventArgs e)
        {
            // If the user is not authorised to see this page or access this function, send them to the error page.
            if (Response.StatusCode == 401 && Request.IsAuthenticated && !Request.Path.SafeContains("/Error/"))
            {
                //Clear the error on the server
                Response.Clear();

                // Setup route to Error Controller
                var routeValues = new RouteValueDictionary
                {
                    ["controller"] = "Error",
                    ["action"] = "Unauthorized"
                };

                Response.RedirectToRoute("Default", routeValues);
            }
        }
    }
}
