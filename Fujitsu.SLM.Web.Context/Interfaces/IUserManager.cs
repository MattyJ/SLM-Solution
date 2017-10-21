using System.Security.Principal;

namespace Fujitsu.SLM.Web.Context.Interfaces
{
    public interface IUserManager
    {
        IPrincipal User { get; }
        string Name { get; }
        string IsRole();
        bool IsAuthenticated();
        bool HasSLMAdministrator();
        bool HasSLMArchitect();
        bool HasSLMViewer();
        bool IsSLMAdministrator();
        bool IsSLMArchitect();
        bool IsSLMViewer();
    }
}