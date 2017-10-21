using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Models;
using System;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        public ActionResult GeneralError(Exception exception)
        {
            return View(exception);
        }


        public ActionResult SessionExpired()
        {
            return View();
        }


        public ActionResult Unauthorized()
        {
            return View();
        }

        public ActionResult NoRoles(string model)
        {
            return View((object) model);
        }
    }
}