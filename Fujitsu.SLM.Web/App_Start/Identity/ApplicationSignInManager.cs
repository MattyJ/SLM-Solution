using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Services.Interfaces;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Web
{
    // Configure the application sign-in manager which is used in this application.
    [ExcludeFromCodeCoverage]
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        private readonly IParameterService _parameterService;

        public ApplicationSignInManager(ApplicationUserManager userManager,
            IAuthenticationManager authenticationManager,
            IParameterService parameterService)
            : base(userManager, authenticationManager)
        {
            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }
            if (authenticationManager == null)
            {
                throw new ArgumentNullException("authenticationManager");
            }
            if (parameterService == null)
            {
                throw new ArgumentNullException("parameterService");
            }
            _parameterService = parameterService;
        }

        public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
        {
            // Before attempting to login, check if the user exists and if do they do, whether they should be logged out.

            var now = DateTime.UtcNow;

            // Get user inactivity lockout period.
            var lockoutPeriodDays = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.UserInactivityLockoutDays);
            var lockoutPeriodDate = now.AddDays(lockoutPeriodDays * -1);

            // Get current user.
            var user = await UserManager.FindByNameAsync(userName);

            // Check if the current user has been inactive for too long.
            if (user == null)
            {
                return SignInStatus.Failure;
            }

            if (!UserManager.IsInRole(user.Id, "Administrator") && user.LastLoginUtc < lockoutPeriodDate)
            {
                // The user needs to be locked out.
                user.LockoutEndDateUtc = DateTime.MaxValue;
                await UserManager.UpdateAsync(user);

                return SignInStatus.LockedOut;
            }

            var loginResult = await base.PasswordSignInAsync(userName, password, isPersistent, shouldLockout);
            if (loginResult == SignInStatus.Success)
            {
                user.LastLoginUtc = now;
                await UserManager.UpdateAsync(user);
            }
            return loginResult;
        }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        //public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        //{
        //    return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(),
        //        context.Authentication,
        //        context.Get<CacheManager>());
        //}
    }
}
