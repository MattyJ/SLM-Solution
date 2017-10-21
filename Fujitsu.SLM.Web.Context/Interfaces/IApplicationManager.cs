namespace Fujitsu.SLM.Web.Context.Interfaces
{
    public interface IApplicationManager
    {
        /// <summary>
        /// Gets a state object by key.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        object Get(string name);

        /// <summary>
        /// Add a new object to the collection.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        void Add(string name, object value);
    }
}
