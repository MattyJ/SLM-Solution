namespace Fujitsu.Aspose.Spreadsheets.Dependency
{
    public class DependencyResolver
    {
        private static IObjectFactory _objectFactory;

        public T Resolve<T>() where T : class
        {
            return _objectFactory.Resolve<T>();
        }

        public T Resolve<T>(string name) where T : class
        {
            return _objectFactory.Resolve<T>(name);
        }

        public static void SetResolver(IObjectFactory objectFactory)
        {
            _objectFactory = objectFactory;
        }
    }
}
