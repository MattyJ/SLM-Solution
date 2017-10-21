using System.Web.Mvc;
using Fujitsu.SLM.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Extension.Tests
{
    [TestClass]
    public class ModelStateDictionaryExtensionsTests
    {
        private ModelStateDictionary _modelStateDictionary;
        private const string Error1Key = "Error1Key";
        private const string Error1Message = "Error1Message";
        private const string Error2Key = "Error2Key";
        private const string Error2Message = "Error2Message";

        [TestInitialize]
        public void TestInitilize()
        {
            this._modelStateDictionary = new ModelStateDictionary();
            this._modelStateDictionary.AddModelError(Error1Key, Error1Message);
            this._modelStateDictionary.AddModelError(Error2Key, Error2Message);
        }

        [TestMethod]
        public void ModelStateDictionaryExtensions_GetModelStateMesssages_ModelStateHasNoMessages_ReturnsEmptyList()
        {
            var messages = new ModelStateDictionary().GetModelStateMesssages();
            Assert.IsNotNull(messages);
            Assert.AreEqual(0, messages.Count);
        }

        [TestMethod]
        public void ModelStateDictionaryExtensions_GetModelStateMesssages_ModelStateHasMultipleMessages_ReturnsAll()
        {
            var messages = this._modelStateDictionary.GetModelStateMesssages();
            Assert.AreEqual(2, messages.Count);
            Assert.AreEqual(Error1Message, messages[0]);
            Assert.AreEqual(Error2Message, messages[1]);
        }
    }
}