using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.ModelHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Extension.Tests
{
    [TestClass]
    public class ServiceComponentExtensionsTests
    {
        [TestMethod]
        public void ServiceComponentHelper_ServiceComponentHelper_ServiceComponentIsNull_ReturnsHelper()
        {
            var sc = null as ServiceComponent;
            var helper = sc.ServiceComponentHelper();
            Assert.IsInstanceOfType(helper, typeof(IServiceComponentHelper));
        }

        [TestMethod]
        public void ServiceComponentHelper_ServiceComponentHelper_ServiceComponentIsNotNull_ReturnsHelper()
        {
            var sc = new ServiceComponent();
            var helper = sc.ServiceComponentHelper();
            Assert.IsInstanceOfType(helper, typeof(IServiceComponentHelper));
        }
    }
}