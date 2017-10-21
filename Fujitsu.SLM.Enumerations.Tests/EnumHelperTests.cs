using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Fujitsu.SLM.Enumerations.Tests
{
    [TestClass]
    public class EnumHelperTests
    {
        [TestMethod]
        public void EnumHelper_GetAttributeOfType_EnumHasAttribute_ReturnsDescription()
        {
            var att = ServiceComponentLevel.Level1.GetAttributeOfType<System.ComponentModel.DescriptionAttribute>();
            Assert.AreEqual("Level 1", att.Description);
        }

        [TestMethod]
        public void EnumHelper_AsSelectListItems_UseEnumValues_ReturnsCorrectNumber()
        {
            var list = EnumExtensions.AsSelectListItems<ServiceComponentLevel>();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void EnumHelper_AsSelectListItems_UseEnumValues_HasCorrectValues()
        {
            var list = EnumExtensions.AsSelectListItems<ServiceComponentLevel>();
            Assert.IsTrue(list.Any(x => x.Text == "Level 1"));
            Assert.IsTrue(list.Any(x => x.Text == "Level 2"));
            Assert.IsTrue(list.Any(x => x.Value == "Level1"));
            Assert.IsTrue(list.Any(x => x.Value == "Level2"));
        }

        [TestMethod]
        public void EnumHelper_AsSelectListItems_UseEnumValuesWithIntFlag_ReturnsCorrectNumber()
        {
            var list = EnumExtensions.AsSelectListItems<ServiceComponentLevel>(true);
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void EnumHelper_AsSelectListItems_UseEnumValuesWityhIntFlag_HasCorrectValues()
        {
            var list = EnumExtensions.AsSelectListItems<ServiceComponentLevel>(true);
            Assert.IsTrue(list.Any(x => x.Text == "Level 1"));
            Assert.IsTrue(list.Any(x => x.Text == "Level 2"));
            Assert.IsTrue(list.Any(x => x.Value == "1"));
            Assert.IsTrue(list.Any(x => x.Value == "2"));
        }

        [TestMethod]
        public void EnumHelper_ConvertEnumAndDescriptionToList_GetList_ReturnsCorrectNumber()
        {
            var list = EnumExtensions.ConvertEnumAndDescriptionToList<ServiceComponentLevel>();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void EnumHelper_ConvertEnumAndDescriptionToList_GetList_HasCorrectValues()
        {
            var list = EnumExtensions.ConvertEnumAndDescriptionToList<ServiceComponentLevel>();
            Assert.IsTrue(list.Any(x => x.Key == "Level1"));
            Assert.IsTrue(list.Any(x => x.Key == "Level2"));
            Assert.IsTrue(list.Any(x => x.Value == "Level 1"));
            Assert.IsTrue(list.Any(x => x.Value == "Level 2"));
        }

        [TestMethod]
        public void EnumHelper_ConvertEnumToList_GetList_ReturnsCorrectNumber()
        {
            var list = EnumExtensions.ConvertEnumToList<ServiceComponentLevel>();
            Assert.AreEqual(2, list.Count);
        }

        [TestMethod]
        public void EnumHelper_ConvertEnumToList_GetList_HasCorrectValues()
        {
            var list = EnumExtensions.ConvertEnumToList<ServiceComponentLevel>();
            Assert.IsTrue(list.Contains("Level1"));
            Assert.IsTrue(list.Contains("Level2"));
        }

        [TestMethod]
        public void EnumHelper_GetEnumDescription_GetValue_HasCorrectValue()
        {
            var d = EnumExtensions.GetEnumDescription(ServiceComponentLevel.Level2);
            Assert.AreEqual("Level 2", d);
        }

        [TestMethod]
        public void EnumHelper_ToEnum_EnumStringValueSupplied_HasCorrectValue()
        {
            var e = "Level2".ToEnum<ServiceComponentLevel>();
            Assert.AreEqual(ServiceComponentLevel.Level2, e);
        }

        [TestMethod]
        public void EnumHelper_ToEnumText_IntIsEnum_HasCorrectValue()
        {
            var e = 2.ToEnumText<ServiceComponentLevel>();
            Assert.AreEqual("Level2", e);
        }
    }
}
