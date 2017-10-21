using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data;
using Microsoft.AspNet.Identity;

namespace Fujitsu.SLM.Web.Extensions
{
    public static class UserExtensions
    {
        public static ApplicationUser GetApplicationUser(this IPrincipal userPrincipal,
            ApplicationUserManager userManager)
        {
            return userManager.FindById(userPrincipal.Identity.GetUserId());
        }

        public static string GetApplicationUserId(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            return userManager.FindById(userPrincipal.Identity.GetUserId()).Id;
        }


        public static bool IsSLMAdministrator(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            return userManager.IsInRole(userPrincipal.GetApplicationUserId(userManager), UserRoles.Administrator);
        }

        public static bool IsSLMArchitect(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            return !IsSLMAdministrator(userPrincipal, userManager) &&
                   userManager.IsInRole(userPrincipal.GetApplicationUserId(userManager), UserRoles.Architect);
        }

        public static bool IsSLMViewer(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            return !IsSLMArchitect(userPrincipal, userManager) &&
                   userManager.IsInRole(userPrincipal.GetApplicationUserId(userManager), UserRoles.Viewer);
        }

        public static bool HasSLMAdministrator(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            return userManager.IsInRole(userPrincipal.GetApplicationUserId(userManager), UserRoles.Administrator);
        }

        public static bool HasSLMArchitect(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            return userManager.IsInRole(userPrincipal.GetApplicationUserId(userManager), UserRoles.Architect);
        }

        public static bool HasSLMViewer(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            return userManager.IsInRole(userPrincipal.GetApplicationUserId(userManager), UserRoles.Viewer);
        }


        public static string IsRole(this IPrincipal userPrincipal, ApplicationUserManager userManager)
        {
            if (IsSLMAdministrator(userPrincipal, userManager))
            {
                return UserRoles.Administrator;
            }
            return IsSLMArchitect(userPrincipal, userManager)
                ? UserRoles.Architect
                : UserRoles.Viewer;
        }
    }
}
