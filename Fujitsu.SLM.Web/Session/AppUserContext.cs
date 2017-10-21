using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Web.Context.Interfaces;
using Fujitsu.SLM.Web.Interfaces;
using Fujitsu.SLM.Web.Models.Session;
using AppContext = Fujitsu.SLM.Web.Models.Session.AppContext;

namespace Fujitsu.SLM.Web.Session
{
    public class AppUserContext : IAppUserContext
    {
        public AppUserContext(IContextManager contextManager)
        {
            if (contextManager == null)
            {
                throw new ArgumentNullException("contextManager");
            }

            this.Current = contextManager.SessionManager.Get<AppContext>(SessionNames.AppContext);
            if (this.Current == null)
            {
                this.Current = new AppContext();
                contextManager.SessionManager.Add(SessionNames.AppContext, this.Current);
            }
        }

        public AppContext Current { get; private set; }

        /// <summary>
        /// WARNING: Please only use this factory method from within the views. For other instances, standard
        /// Unity injection should be utilised.
        /// </summary>
        /// <returns></returns>
        public static IAppUserContext GetAppUserContext()
        {
            var builder = new ObjectBuilder();
            return builder.Resolve<IAppUserContext>();
        }
    }

}
