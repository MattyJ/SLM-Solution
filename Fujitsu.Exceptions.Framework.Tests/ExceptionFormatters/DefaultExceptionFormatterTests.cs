using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Fujitsu.Exceptions.Framework.Tests.ExceptionFormatters
{
    [TestClass]
    public class DefaultExceptionFormatterTests
    {
        private IObjectBuilder _objectBuilder;

        [TestInitialize]
        public void TestIntialize()
        {
            _objectBuilder = new ObjectBuilder(ObjectBuilderHelper.SetupObjectBuilder);
        }

        [TestMethod]
        public void DefaultExceptionFormatter_ToString_ExceptionMessage_MessageAndStackTraceReturned()
        {
            string message;
            try
            {
                throw new Exception("Foobar");
            }
            catch (Exception ex)
            {
                message = ex.Flatten();
            }
            Assert.IsTrue(message.Contains("Foobar"));
            Assert.IsTrue(message.Contains("DefaultExceptionFormatterTests"));
        }

        [TestMethod]
        public void DefaultExceptionFormatter_ToString_ExceptionMessage_InnerExceptionProcessed()
        {
            string message;
            try
            {
                throw new Exception("Foobar", new Exception("InnerException"));
            }
            catch (Exception ex)
            {
                message = ex.Flatten();
            }
            Assert.IsTrue(message.Contains("Foobar"));
            Assert.IsTrue(message.Contains("InnerException"));
            Assert.IsTrue(message.Contains("DefaultExceptionFormatterTests"));
        }
    }
}