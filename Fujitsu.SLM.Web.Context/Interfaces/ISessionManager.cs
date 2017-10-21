using System;

namespace Fujitsu.SLM.Web.Context.Interfaces
{
    public interface ISessionManager
    {
        // <summary>
        /// Points to a uniquie guid for this session.
        /// </summary>
        Guid SessionGuid { get; }

        /// <summary>
        /// Adds an entry into the Session.
        /// </summary>
        /// <param name="key">The key of the object being added.</param>
        /// <param name="value">The object to add.</param>
        void Add(string key, object value);

        /// <summary>
        /// Deletes an item from Session.
        /// </summary>
        /// <param name="key">The key of the item to be deleted.</param>
        void Remove(string key);

        /// <summary>
        /// Get an entry from the Session.
        /// </summary>
        /// <param name="key">The key of the item.</param>
        /// <returns>The retrieved object - null if not present.</returns>
        object Get(string key);

        /// <summary>
        /// Get an entry from the Session.
        /// </summary>
        /// <typeparam name="T">The Type of the object to be returned.</typeparam>
        /// <param name="key">The key of the item.</param>
        /// <returns>The retrieved object - null if not present.</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Cancels the current session.
        /// </summary>
        void Abandon();
    }
}
