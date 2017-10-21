using System.Collections.Generic;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.ModelHelpers.Tests
{
    [TestClass]
    public class ServiceComponentHelperTests
    {
        private IServiceComponentHelper _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new ServiceComponentHelper();
        }

        [TestMethod]
        public void ServiceComponentHelper_CanDelete_ServiceComponentIsNull_CannotDelete()
        {
            var component = null as ServiceComponent;
            Assert.IsFalse(_target.CanDelete(component));
        }

        [TestMethod]
        public void ServiceComponentHelper_CanDelete_ChildServiceComponentNotNull_CannotDelete()
        {
            var component = new ServiceComponent
            {
                ChildServiceComponents = new List<ServiceComponent>(),
            };

            component.ChildServiceComponents.Add(new ServiceComponent());
            Assert.IsFalse(_target.CanDelete(component));
        }

        [TestMethod]
        public void ServiceComponentHelper_CanDelete_ResolverHasValue_CannotDelete()
        {
            var component = new ServiceComponent
            {
                Resolver = new Resolver()
            };
            Assert.IsFalse(_target.CanDelete(component));
        }

        [TestMethod]
        public void ServiceComponentHelper_CanDelete_ChildServiceComponentAndResolverGroupAndResolverTypeAndServiceDeliveryUnitNull_CanDelete()
        {
            var component = new ServiceComponent();
            Assert.IsTrue(_target.CanDelete(component));
        }

        [TestMethod]
        public void ServiceComponentHelper_GetEditState_ComponentIsLevel2_EditStateLevel2()
        {
            var sc = new ServiceComponent{ComponentLevel = (int) ServiceComponentLevel.Level2};
            var state = _target.GetEditState(sc);
            Assert.AreEqual(ServiceComponentEditState.Level2, state);
        }

        [TestMethod]
        public void ServiceComponentHelper_GetEditState_ComponentHasNoChildComponentOrResolverAndIsLevel1_EditStateLevel1WithNoChildComponentOrResolver()
        {
            var sc = new ServiceComponent { ComponentLevel = (int)ServiceComponentLevel.Level1 };
            var state = _target.GetEditState(sc);
            Assert.AreEqual(ServiceComponentEditState.Level1WithNoChildComponentOrResolver, state);
        }

        [TestMethod]
        public void ServiceComponentHelper_GetEditState_ComponentHasEmptyChildComponentOrResolverAndIsLevel1_EditStateLevel1WithNoChildComponentOrResolver()
        {
            var sc = new ServiceComponent
            {
                ComponentLevel = (int)ServiceComponentLevel.Level1,
                ChildServiceComponents = new List<ServiceComponent>()
            };

            var state = _target.GetEditState(sc);

            Assert.AreEqual(ServiceComponentEditState.Level1WithNoChildComponentOrResolver, state);
        }

        [TestMethod]
        public void ServiceComponentHelper_GetEditState_ComponentHasChildComponentAndIsLevel1_EditStateLevel1WithChildComponent()
        {
            var sc = new ServiceComponent
            {
                ComponentLevel = (int)ServiceComponentLevel.Level1,
                ChildServiceComponents = new List<ServiceComponent> {  new ServiceComponent { Id = 4567,
                    ComponentLevel = (int) ServiceComponentLevel.Level2, ComponentName = "Test"} }
            };

            var state = _target.GetEditState(sc);

            Assert.AreEqual(ServiceComponentEditState.Level1WithChildComponent, state);
        }

        [TestMethod]
        public void ServiceComponentHelper_GetEditState_ComponentHasResolverAndIsLevel1_EditStateLevel1WithResolver()
        {
            var sc = new ServiceComponent
            {
                ComponentLevel = (int)ServiceComponentLevel.Level1,
                Resolver = UnitTestHelper.GenerateRandomData<Resolver>(),
            };
            var state = _target.GetEditState(sc);
            Assert.AreEqual(ServiceComponentEditState.Level1WithResolver, state);
        }
    }
}