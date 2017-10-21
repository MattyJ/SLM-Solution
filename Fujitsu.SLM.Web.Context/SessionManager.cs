using System;
using System.Web;
using Fujitsu.SLM.Web.Context.Interfaces;

namespace Fujitsu.SLM.Web.Context
{
    public class SessionManager : ISessionManager
    {
        private readonly HttpSessionStateBase _session;

        private const string SessionGuidKey = "SessionGuid";

        /// <summary>
        /// Create a new SessionManager object. This is used to manage the Session State.
        /// </summary>
        /// <param name="session"></param>
        public SessionManager(HttpSessionStateBase session)
        {
            this._session = session;
        }

        public Guid SessionGuid
        {
            get
            {
                var guid = this.Get<string>(SessionGuidKey);

                if (string.IsNullOrEmpty(guid))
                {
                    guid = Guid.NewGuid().ToString();

                    this.Add(SessionGuidKey, guid);
                }

                return new Guid(guid);
            }
        }

        /// <summary>
        /// Adds an entry into the Session.
        /// </summary>
        /// <param name="key">The key of the object being added.</param>
        /// <param name="value">The object to add.</param>
        public void Add(string key, object value)
        {
            this._session.Add(key, value);
        }

        /// <summary>
        /// Deletes an item from Session.
        /// </summary>
        /// <param name="key">The key of the item to be deleted.</param>
        public void Remove(string key)
        {
            this._session.Remove(key);
        }

        /// <summary>
        /// Get an entry from the Session.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns>The retrieved object - null if not present.</returns>
        public object Get(string key)
        {
            return this._session[key];
        }

        /// <summary>
        /// Get an entry from the Session.
        /// </summary>
        /// <typeparam name="T">The Type of the object to be returned.</typeparam>
        /// <param name="key">The key of the item.</param>
        /// <returns>The retrieved object - null if not present.</returns>
        public T Get<T>(string key) where T : class
        {
            return this._session[key] as T;
        }

        /// <summary>
        /// Cancels the current session.
        /// </summary>
        public void Abandon()
        {
            this._session.Abandon();
        }

        /// <summary>
        /// Removes and keys and values from the Session.
        /// </summary>
        public void Clear()
        {
            this._session.Clear();
        }
    }
}
