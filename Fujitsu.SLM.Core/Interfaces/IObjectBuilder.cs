using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace Fujitsu.SLM.Core.Interfaces
{
    public interface IObjectBuilder
    {
        IUnityContainer GetContainer();
        T Resolve<T>();
        T Resolve<T>(string name);
        T Resolve<T>(string name, bool allowDefault);
        void AddChildContainer();
        void RemoveChildContainer();
        void RemoveAllChildContainers();
    }
}
