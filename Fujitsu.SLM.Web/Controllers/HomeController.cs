using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models;
using System;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private readonly IAppUserContext _appUserContext;
        private readonly ICacheManager _cacheManager;
        private readonly IContextManager _contextManager;
        private readonly IUserManager _userManager;
        private readonly IParameterService _parameterService;

        public HomeController(IAppUserContext appUserContext,
            ICacheManager cacheManager,
            IContextManager contextManager,
            IUserManager userManager,
            IParameterService parameterService)
        {
            if (parameterService == null)
            {
                throw new ArgumentNullException();
            }

            if (appUserContext == null)
            {
                throw new ArgumentNullException(nameof(appUserContext));
            }

            if (cacheManager == null)
            {
                throw new ArgumentNullException(nameof(cacheManager));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }
            _appUserContext = appUserContext;
            _cacheManager = cacheManager;
            _contextManager = contextManager;
            _userManager = userManager;
            _parameterService = parameterService;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            var homePageViewModel = new HomePageViewModel
            { SupportEmailAddress = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ContactEmailAddress) };

            if (_userManager.IsAuthenticated() && _userManager.IsRole().Equals(UserRoles.None))
            {
                return RedirectToAction("NoRoles", "Error", new { model = homePageViewModel.SupportEmailAddress });
            }

            return View(homePageViewModel);
        }

        public PartialViewResult CurrentCustomer()
        {
            var currentCustomer = _appUserContext.Current.CurrentCustomer;

            return PartialView("_CurrentCustomer", currentCustomer);
        }

        [HttpGet]
        public ActionResult Roadmap()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            return View();
        }

        [HttpGet]
        public ActionResult ReleaseHistory()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            return View();
        }

        [HttpGet]
        public ActionResult WhatsNew()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            return View();
        }

        [HttpGet]
        public ActionResult About()
        {
            if (_appUserContext.Current != null)
            {
                // Reset the Application User Context
                _appUserContext.Current.CurrentCustomer = new CurrentCustomerViewModel();
            }

            return View();
        }

    }
}