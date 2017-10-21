using Microsoft.Practices.Unity;

namespace Fujitsu.SLM.UnitTesting
{
    public static class ObjectBuilderHelper
    {
        public static void SetupObjectBuilder(IUnityContainer container)
        {
            Exceptions.Framework.UnityConfig.RegisterTypes(container, () => new HierarchicalLifetimeManager());
        }
    }
}
