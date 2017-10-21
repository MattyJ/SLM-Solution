using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Core.Injection;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Core.Tests
{
    [TestClass]
    public class ObjectBuilderTests
    {
        [TestMethod]
        public void ObjectBuilder_Constructor_NoParameters_ReturnsInstance()
        {
            var ob = new ObjectBuilder();
            Assert.IsNotNull(ob);
        }

        [TestMethod]
        public void ObjectBuilder_Constructor_ContainerRegisterParameter_ReturnsInstance()
        {
            var ob = new ObjectBuilder();
            Assert.IsNotNull(ob);
            var registered = false;
            try
            {
                var instance = ob.Resolve<IObjectBuilderTestClass>();
                if (instance != null)
                {
                    registered = true;
                }
            }
            catch
            {
            }
            Assert.IsTrue(registered);
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<IObjectBuilderTestClass, ObjectBuilderTestClass>();
        }
    }

    public interface IObjectBuilderTestClass
    {
        string TestProperty { get; set; }
        string InstanceId { get; }
    }

    public class ObjectBuilderTestClass : IObjectBuilderTestClass
    {
        public ObjectBuilderTestClass()
        {
            InstanceId = Guid.NewGuid().ToString();
        }
        public string TestProperty { get; set; }
        public string InstanceId { get; private set; }
    }
}
