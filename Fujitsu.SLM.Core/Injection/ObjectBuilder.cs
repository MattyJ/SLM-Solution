using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Core.Interfaces;
using Microsoft.Practices.Unity;

namespace Fujitsu.SLM.Core.Injection
{
    public class ObjectBuilder : IObjectBuilder
    {
        private static readonly Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            container.RegisterTypes(
                AllClasses.FromAssembliesInBasePath(),
                WithMappings.FromMatchingInterface,
                WithName.Default,
                WithLifetime.PerResolve);
            return container;
        });

        private readonly Stack<IUnityContainer> _childContainers = new Stack<IUnityContainer>();

        public ObjectBuilder()
        {
        }

        public ObjectBuilder(params Action<IUnityContainer>[] registers)
        {
            foreach (var register in registers)
            {
                register(Container.Value);
            }
        }

        public void AddChildContainer()
        {
            this._childContainers.Push(Container.Value.CreateChildContainer());
        }

        public void RemoveChildContainer()
        {
            if (this._childContainers.Count > 0)
            {
                var container = this._childContainers.Pop();
                container.Dispose();
            }
        }

        public void RemoveAllChildContainers()
        {
            while (this._childContainers.Count > 0)
            {
                var container = this._childContainers.Pop();
                container.Dispose();
            }
        }

        public IUnityContainer GetContainer()
        {
            return Container.Value;
        }

        public T Resolve<T>()
        {
            if (this._childContainers.Count > 0)
            {
                var container = this._childContainers.Peek();
                return container.Resolve<T>();
            }
            return Container.Value.Resolve<T>();
        }

        public T Resolve<T>(string name)
        {
            return this.Resolve<T>(name, false);
        }

        public T Resolve<T>(string name, bool allowDefault)
        {
            var container = Container.Value;
            if (this._childContainers.Count > 0)
            {
                container = this._childContainers.Peek();
            }
            if (container.IsRegistered<T>(name))
            {
                return container.Resolve<T>(name);
            }
            if (allowDefault && container.IsRegistered<T>())
            {
                return container.Resolve<T>();
            }
            return container.Resolve<T>(name);
        }
    }
}
