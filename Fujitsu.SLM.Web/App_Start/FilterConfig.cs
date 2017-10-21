using Fujitsu.SLM.Web.Attributes;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web
{
    [ExcludeFromCodeCoverage]
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionExpiryAttribute());
            filters.Add(new AuthorizeAttribute());
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}
