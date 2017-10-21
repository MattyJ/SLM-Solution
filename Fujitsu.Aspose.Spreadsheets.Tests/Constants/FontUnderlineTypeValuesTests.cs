using System;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests.Constants
{
    [TestClass]
    public class FontUnderlineTypeValuesTests
    {
        [TestMethod]
        public void FontUnderlineTypeValues_ParseToAsposeEnum_AllValuesParse()
        {
            #region Assert

            FontUnderlineType e;
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Accounting, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Dash, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DashDotDotHeavy, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DashDotHeavy, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DashLong, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DashLongHeavy, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DashedHeavy, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DotDash, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DotDotDash, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Dotted, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DottedHeavy, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Double, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.DoubleAccounting, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Heavy, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.None, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Single, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Wave, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.WavyDouble, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.WavyHeavy, out e));
            Assert.IsTrue(Enum.TryParse(FontUnderlineTypeValues.Words, out e));

            #endregion
        }
    }
}