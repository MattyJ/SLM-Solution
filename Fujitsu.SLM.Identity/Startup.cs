using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Fujitsu.SLM.Identity.Startup))]
namespace Fujitsu.SLM.Identity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
