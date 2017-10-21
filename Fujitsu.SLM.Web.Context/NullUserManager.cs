using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Web.Context.Interfaces;
using System.Security.Principal;
using System.Web;

namespace Fujitsu.SLM.Web.Context
{
    public class NullUserManager : IUserManager, IUserIdentity
    {
        private readonly HttpRequestBase _request;

        public NullUserManager()
        {
            var context = new HttpContextWrapper(HttpContext.Current);
            _request = context.Request;

        }

        public NullUserManager(IPrincipal user, HttpRequestBase request)
        {
            User = user;
            _request = request;
        }

        public IPrincipal User { get; private set; }

        public string Name
        {
            get
            {
                return string.Empty;
            }
        }

        public string IsRole()
        {
            return UserRoles.None;
        }

        public bool HasSLMAdministrator()
        {
            return false;
        }

        public bool HasSLMArchitect()
        {
            return false;
        }

        public bool HasSLMViewer()
        {
            return false;
        }

        public bool IsSLMAdministrator()
        {
            return false;
        }

        public bool IsSLMArchitect()
        {
            return false;
        }

        public bool IsSLMViewer()
        {
            return false;
        }

        public bool IsAuthenticated()
        {
            return false;
        }
    }
}
