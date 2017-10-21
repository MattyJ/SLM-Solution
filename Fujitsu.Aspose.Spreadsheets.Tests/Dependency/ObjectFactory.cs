using System;
using Fujitsu.Aspose.Spreadsheets.Dependency;

namespace Fujitsu.Aspose.Spreadsheets.Tests.Dependency
{
    public class ObjectFactory : IObjectFactory
    {
        public T Resolve<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public T Resolve<T>(string name) where T : class
        {
            switch (name)
            {
                case InjectorNames.CurrentMonthYear:
                    return new CurrentMonthYear() as T;
                default:
                    throw new Exception("Foobar");
            }
        }
    }
}