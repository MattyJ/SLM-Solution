using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Services.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Extensions;
using Fujitsu.SLM.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Controllers
{
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class AccountController : Controller
    {
        private readonly ApplicationSignInManager _signInManager;
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IContextManager _contextManager;
        private readonly IParameterService _parameterService;

        public AccountController(ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IAuthenticationManager authenticationManager,
            IContextManager contextManager,
            IParameterService parameterService)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException(nameof(userManager));
            }

            if (signInManager == null)
            {
                throw new ArgumentNullException(nameof(signInManager));
            }

            if (authenticationManager == null)
            {
                throw new ArgumentNullException(nameof(authenticationManager));
            }

            if (contextManager == null)
            {
                throw new ArgumentNullException(nameof(contextManager));
            }

            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationManager = authenticationManager;
            _contextManager = contextManager;
            _parameterService = parameterService;
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var loggedinUser = await _userManager.FindAsync(model.Email, model.Password);
            if (loggedinUser != null)
            {
                // Now user have entered correct username and password.
                // Time to change the security stamp
                await _userManager.UpdateSecurityStampAsync(loggedinUser.Id);
            }

            // To disable password failures to trigger account lockout, change to shouldLockout: false
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: true);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    RegionTypeId = model.RegionTypeId,
                    LastLoginUtc = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }

                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            _authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ViewResult MyIdentity()
        {
            var user = _contextManager.UserManager.User.GetApplicationUser(_userManager);

            var claimsPrincipal = _contextManager.UserManager.User as ClaimsPrincipal;

            string supportEmail;
            try
            {
                supportEmail = _parameterService.GetParameterByNameAndCache<string>(ParameterNames.ContactEmailAddress);
            }
            catch
            {
                supportEmail = "[Parameter not found]";
            }

            var model = new MyIdentityViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserRoles = _userManager.GetRoles(user.Id).OrderBy(x => x).ToList(),
                Claims = claimsPrincipal.Claims.Where(x => !x.Type.EndsWith("sid")).OrderBy(x => x.Type)
                    .ToList(),
                SupportEmailAddress = supportEmail
            };

            return View(model);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {


            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion


    }
}