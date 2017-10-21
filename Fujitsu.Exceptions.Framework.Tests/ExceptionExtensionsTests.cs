using Fujitsu.Exceptions.Framework.Interfaces;
using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fujitsu.Exceptions.Framework.Tests
{
    [TestClass]
    public class ExceptionExtensionsTests
    {
        private IObjectBuilder _objectBuilder;
        private const string ExceptionMessage = "Oh Foobar!!";

        [TestInitialize]
        public void TestInitialise()
        {
            _objectBuilder = new ObjectBuilder(ObjectBuilderHelper.SetupObjectBuilder);
        }

        [TestMethod]
        public void ExceptionExtensions_Flatten_ExceptionIsNull_ReturnsEmptyString()
        {
            var ex = null as Exception;
            Assert.AreEqual(string.Empty, ex.Flatten());
        }

        [TestMethod]
        public void ExceptionExtensions_Flatten_ExceptionRegisteredInObjectBuilder_MessageReturned()
        {
            var ex = new Exception(ExceptionMessage);
            var message = ex.Flatten();
            Assert.IsNotNull(message);
            Assert.IsTrue(message.Contains(ExceptionMessage));
        }

        [TestMethod]
        public void ExceptionExtensions_Flatten_ExceptionThrownFromFormatter_MessageReturned()
        {
            _objectBuilder.GetContainer().RegisterType<IExceptionFormatter, TestExceptionFormatter>();
            var ex = new Exception(ExceptionMessage);
            var message = ex.Flatten();
            Assert.IsNotNull(message);
            Assert.IsTrue(message.StartsWith("Failed to format exception"));
        }

        public class TestExceptionFormatter : IExceptionFormatter
        {
            public string ToString(Exception exception)
            {
                throw new NotImplementedException();
            }
        }
    }
}