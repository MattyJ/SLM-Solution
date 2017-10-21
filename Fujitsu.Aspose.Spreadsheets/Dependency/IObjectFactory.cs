namespace Fujitsu.Aspose.Spreadsheets.Dependency
{
    public interface IObjectFactory
    {
        T Resolve<T>() where T : class;
        T Resolve<T>(string name) where T : class;
    }
}
