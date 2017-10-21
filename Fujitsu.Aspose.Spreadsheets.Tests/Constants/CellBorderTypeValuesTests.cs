using System;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests.Constants
{
    [TestClass]
    public class CellBorderTypeValuesTests
    {
        [TestMethod]
        public void CellBorderTypeValues_ParseToAsposeEnum_AllValuesParse()
        {
            #region Assert

            CellBorderType e;
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.DashDot, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.DashDotDot, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.Dashed, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.Dotted, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.Double, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.Hair, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.Medium, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.MediumDashDot, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.MediumDashDotDot, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.MediumDashed, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.None, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.SlantedDashDot, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.Thick, out e));
            Assert.IsTrue(Enum.TryParse(CellBorderTypeValues.Thin, out e));

            #endregion
        }
    }
}