using Fujitsu.SLM.Core.Injection;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace Fujitsu.Exceptions.Framework.Tests.ExceptionFormatters
{
    [TestClass]
    public class DbEntityValidationExceptionFormatterTests
    {
        private IObjectBuilder _objectBuilder;
        private DbEntityValidationException _dbEntityValidationException;

        [TestInitialize]
        public void TestIntialize()
        {
            _objectBuilder = new ObjectBuilder(ObjectBuilderHelper.SetupObjectBuilder);

            _dbEntityValidationException = new DbEntityValidationException("Foobar", new List<DbEntityValidationResult>());
        }

        [TestMethod]
        public void DbEntityValidationExceptionFormatter_ToString_ExceptionMessage_MessageAndStackTraceReturned()
        {
            var message = string.Empty;
            try
            {
                throw _dbEntityValidationException;
            }
            catch (Exception ex)
            {
                message = ex.Flatten();
            }
            Assert.IsTrue(message.Contains("Foobar"));
            Assert.IsTrue(message.Contains("DbEntityValidationExceptionFormatterTests"));
        }
    }
}