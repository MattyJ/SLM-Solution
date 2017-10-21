using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.SLM.Extension.Tests
{
    [TestClass]
    public class ObjectExtensionsTests
    {
        private const string PropertyNameString = "TestProperty";
        private const string PropertyValueString = "Project X";

        private const string PropertyNameDecimal = "AnotherTestProperty";
        private const decimal PropertyValueDecimal = 100.56m;

        [TestMethod]
        public void ObjectExtensions_GetClassAttributeValue_ObjectIsNull_ReturnsNull()
        {
            TestClass c = null;
            Assert.IsNull(c.GetClassAttributeValue((DescriptionAttribute a) => a.Description));
        }

        [TestMethod]
        public void ObjectExtensions_GetClassAttributeValue_ObjectHasAttribute_ReturnsAttributeValue()
        {
            var c = new TestClass();
            Assert.IsNotNull(c.GetClassAttributeValue((ExcludeFromCodeCoverageAttribute a) => a.TypeId));
        }

        [TestMethod]
        public void ObjectExtensions_GetMethodAttributeValue_ObjectNull_ReturnsNull()
        {
            TestClass c = null;
            Assert.IsNull(c.GetMethodAttributeValue("TestMethod", (DescriptionAttribute a) => a.Description));
        }

        [TestMethod]
        public void ObjectExtensions_GetMethodAttributeValue_ObjectHasAttribute_ReturnsAttributeValue()
        {
            var c = new TestClass();
            Assert.AreEqual("XXX", c.GetMethodAttributeValue("TestMethod", (DescriptionAttribute a) => a.Description));
        }

        [TestMethod]
        public void ObjectExtensions_SetProperty_InstanceIsNull_DoesNothingAndReturns()
        {
            var c = null as TestClass;
            c.SetProperty(PropertyNameString, PropertyValueString);
            Assert.IsNull(c);
        }

        [TestMethod]
        public void ObjectExtensions_SetProperty_InstanceInstantiatedSetStringProperty_SetsPropertySuccessfully()
        {
            var c = new TestClass();
            c.SetProperty(PropertyNameString, PropertyValueString);
            Assert.AreEqual(PropertyValueString, c.TestProperty);
        }

        [TestMethod]
        public void ObjectExtensions_SetProperty_InstanceInstantiatedSetDecimalProperty_SetsPropertySuccessfully()
        {
            var c = new TestClass();
            c.SetProperty(PropertyNameDecimal, PropertyValueDecimal);
            Assert.AreEqual(PropertyValueDecimal, c.AnotherTestProperty);
        }

        [TestMethod]
        public void ObjectExtensions_GetProperty_ObjectIsNull_ReturnsNull()
        {
            var c = null as TestClass;
            var r = c.GetProperty("TestProperty");
            Assert.IsNull(r);
        }

        [TestMethod]
        public void ObjectExtensions_GetProperty_PropertyIsPopulated_ReturnsPropertyValues()
        {
            var c = new TestClass() { TestProperty = "Project X" };
            var r = c.GetProperty("TestProperty");
            Assert.AreEqual("Project X", r);
        }

        [TestMethod]
        public void ObjectExtensions_GetProperties_ObjectIsNull_ReturnsNull()
        {
            var c = null as TestClass;
            var r = c.GetProperties(x => x.Name.Contains("ABC"));
            Assert.IsNull(r);
        }

        [TestMethod]
        public void ObjectExtensions_GetProperties_SearchForProperties_ReturnsMatchedValues()
        {
            var c = new TestClass();
            var r = c.GetProperties(x => x.Name.StartsWith("Another")).Select(s => s.Name).OrderBy(o => o).ToList();
            Assert.IsNotNull(r);
            Assert.AreEqual(2, r.Count);
            Assert.AreEqual("AnotherGreatTestProperty", r[0]);
            Assert.AreEqual("AnotherTestProperty", r[1]);
        }

        [TestMethod]
        public void ObjectExtensions_GetPropertyInfo_PropertyOnClass_ReturnsInfo()
        {
            var c = new TestClass();
            var r = c.GetPropertyInfo("TestProperty");
            Assert.IsNotNull(r);
            Assert.AreEqual("TestProperty", r.Name);
        }

        [TestMethod]
        public void ObjectExtensions_ConvertGenericValueToString_SuppliedValueIsNull_ReturnsNull()
        {
            var s = null as string;
            Assert.IsNull(s.ConvertGenericValueToString());
        }

        [TestMethod]
        public void ObjectExtensions_ConvertGenericValueToString_SuppliedValueIsDateTimeFormat_ReturnsFormatAsDefinedByDatabaseConstant()
        {
            var now = DateTime.Now;
            var expected = now.ToString(Database.DateTimeFormat);
            var actual = now.ConvertGenericValueToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ObjectExtensions_ConvertGenericValueToString_SuppliedValueInt_ReturnsInt()
        {
            const string expected = "2";
            var actual = 2.ConvertGenericValueToString();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ObjectExtensions_GetPropertyAttribute_ClassHasAttribute_ReturnsAttributeCorrectType()
        {
            var result = typeof (TestClass).GetPropertyAttribute<TestAttribute>("AnotherGreatTestProperty");
            Assert.IsInstanceOfType(result, typeof(TestAttribute));
        }

        [ExcludeFromCodeCoverage]
        public class TestClass
        {
            public string TestProperty { get; set; }

            public decimal AnotherTestProperty { get; set; }

            [Test(Length=290)]
            public decimal AnotherGreatTestProperty { get; set; }

            [Description("XXX")]
            public void TestMethod()
            {
            }
        }

        public class TestAttribute : Attribute
        {
            public int Length { get; set; }
        }
    }
}