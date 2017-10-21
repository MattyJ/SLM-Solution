using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using System.Security.Principal;
using System.Web;

namespace Fujitsu.SLM.Web.Context
{
    public class UserManager : IUserManager, IUserIdentity
    {
        private readonly HttpRequestBase _request;

        public UserManager()
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            _request = context.Request;
            User = context.User;
        }

        public UserManager(IPrincipal user, HttpRequestBase request)
        {
            User = user;
            _request = request;
        }

        public IPrincipal User { get; private set; }

        public string Name
        {
            get
            {
                var principal = User;
                return principal != null ? principal.Identity.Name : string.Empty;
            }
        }

        public string IsRole()
        {
            if (IsSLMAdministrator())
            {
                return UserRoles.Administrator;
            }
            if (IsSLMArchitect())
            {
                return UserRoles.Architect;
            }
            return IsSLMViewer() ? UserRoles.Viewer : UserRoles.None;
        }

        public bool IsAuthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        public bool HasSLMAdministrator()
        {
            var principal = User;

            return principal != null && principal.IsInRole(UserRoles.Administrator);
        }

        public bool HasSLMArchitect()
        {
            var principal = User;
            return principal != null && principal.IsInRole(UserRoles.Architect);
        }

        public bool HasSLMViewer()
        {
            var principal = User;
            return principal != null && principal.IsInRole(UserRoles.Viewer);
        }

        public bool IsSLMAdministrator()
        {
            var principal = User;
            return principal != null && principal.IsInRole(UserRoles.Administrator);
        }

        public bool IsSLMArchitect()
        {
            var principal = User;
            return principal != null && (!principal.IsInRole(UserRoles.Administrator) &&
                                         principal.IsInRole(UserRoles.Architect));
        }

        public bool IsSLMViewer()
        {
            var principal = User;
            return principal != null && (!principal.IsInRole(UserRoles.Architect) &&
                                         principal.IsInRole(UserRoles.Viewer));
        }
    }
}
