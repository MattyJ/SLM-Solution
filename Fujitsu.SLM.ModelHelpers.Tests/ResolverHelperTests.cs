using Fujitsu.SLM.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Fujitsu.SLM.ModelHelpers.Tests
{
    [TestClass]
    public class ResolverHelperTests
    {
        private IResolverHelper _target;

        [TestInitialize]
        public void Initialize()
        {
            _target = new ResolverHelper();
        }

        [TestMethod]
        public void ResolverHelper_CanDelete_ResolverGroupHasOperationalProcesses_CannotDelete()
        {
            var rg = new Resolver
            {
                OperationalProcessTypes = new List<OperationalProcessType>
                {
                    new OperationalProcessType()
                }
            };
            Assert.IsFalse(_target.CanDelete(rg));
        }

        [TestMethod]
        public void ResolverHelper_CanDelete_ResolverGroupHasNullOperationalProcesses_CanDelete()
        {
            var rg = new Resolver();
            Assert.IsTrue(_target.CanDelete(rg));
        }

        [TestMethod]
        public void ResolverHelper_CanDelete_ResolverGroupHasEmptyOperationalProcesses_CanDelete()
        {
            var rg = new Resolver
            {
                OperationalProcessTypes = new List<OperationalProcessType>()
            };
            Assert.IsTrue(_target.CanDelete(rg));
        }
    }
}