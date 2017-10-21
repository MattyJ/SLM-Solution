using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Attributes
{
    public class AjaxModelErrorStatusModifierAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest() && !filterContext.Controller.ViewData.ModelState.IsValid)
            {
                filterContext.HttpContext.Response.StatusCode = 220;
            }

            base.OnActionExecuted(filterContext);
        }
    }
}
