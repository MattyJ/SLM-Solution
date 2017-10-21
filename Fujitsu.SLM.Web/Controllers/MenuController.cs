using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models.Menu;
using System;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [AllowAnonymous]
    public class MenuController : Controller
    {
        private readonly IContextManager _contextManager;
        private readonly IAppUserContext _appUserContext;
        private readonly ICacheManager _cacheManager;
        private readonly ApplicationUserManager _userManager;

        public MenuController(IContextManager contextManager,
            IAppUserContext appUserContext,
            ICacheManager cacheManager,
            ApplicationUserManager userManager)
        {
            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            if (cacheManager == null)
            {
                throw new ArgumentNullException(nameof(cacheManager));
            }

            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            _contextManager = contextManager;
            _appUserContext = appUserContext;
            _cacheManager = cacheManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public ActionResult HomeMenu()
        {
            return PartialView("_HomeMenu", new HomeMenuModel());
        }

        [AllowAnonymous]
        public ActionResult QuickLinks()
        {
            return PartialView("_QuickLinks");
        }

    }
}