using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Fujitsu.SLM.Extensions;

namespace Fujitsu.SLM.Extension.Tests
{
    [TestClass]
    public class CollectionExtensionsTests
    {
        private ICollection<TestClass> _target;
            
        [TestInitialize]
        public void Initialize()
        {
            _target = new List<TestClass>
            {
                new TestClass() {Title = "A", Delete = true},
                new TestClass() {Title = "A", Delete = true},
                new TestClass() {Title = "A", Delete = false},
                new TestClass() {Title = "A", Delete = true},
                new TestClass() {Title = "B", Delete = false},
            };
        }

        [TestMethod]
        public void CollectionExtensions_RemoveAll_PredicateMatchesSingleCollectionElement_ElementIsRemoved()
        {
            _target.RemoveAll(x => x.Title == "B");
            Assert.AreEqual(4, _target.Count);
            Assert.IsFalse(_target.Any(x => x.Title == "B"));
        }

        [TestMethod]
        public void CollectionExtensions_RemoveAll_PredicateMatchesMultipleCollectionElement_ElementsAreRemoved()
        {
            _target.RemoveAll(x => x.Delete);
            Assert.AreEqual(2, _target.Count);
            Assert.IsFalse(_target.Any(x => x.Delete));
        }

        [TestMethod]
        public void CollectionExtensions_RemoveAll_PredicateMatchesNoCollectionElement_NoElementsAreRemoved()
        {
            _target.RemoveAll(x => x.Title == "XXX");
            Assert.AreEqual(5, _target.Count);
        }
    }

    internal class TestClass
    {
        internal string Title { get; set; }
        internal bool Delete { get; set; }
    }
}
