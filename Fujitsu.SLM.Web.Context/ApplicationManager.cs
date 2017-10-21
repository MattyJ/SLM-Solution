using System.Web;
using Fujitsu.SLM.Web.Context.Interfaces;

namespace Fujitsu.SLM.Web.Context
{
    /// <summary>
    /// A class used to wrap the HttpContext Response object.
    /// </summary>
    public class ApplicationManager : IApplicationManager
    {
        private readonly HttpApplicationStateBase _application;

        /// <summary>
        /// Create a Application wrapper.
        /// </summary>
        /// <param name="application"></param>
        public ApplicationManager(HttpApplicationStateBase application)
        {
            this._application = application;
        }

        /// <summary>
        /// Gets a state object by key.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object Get(string name)
        {
            return this._application.Get(name);
        }

        /// <summary>
        /// Add a new object to the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Add(string name, object value)
        {
            this._application.Add(name, value);
        }
    }
}
