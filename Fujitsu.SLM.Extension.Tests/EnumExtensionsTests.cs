using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Extension.Tests
{
    [TestClass]
    public class EnumExtensionsTests
    {
        [TestMethod]
        public void EnumExtensions_GetEnumIntFromText_EnumValueIsOne_ReturnsOne()
        {
            const string level = "LevelOne";
            Assert.AreEqual(1, level.GetEnumIntFromText<LevelName>());
        }
    }
}