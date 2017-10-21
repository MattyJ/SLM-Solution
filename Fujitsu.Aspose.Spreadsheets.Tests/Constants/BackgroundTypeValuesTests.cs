using System;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests.Constants
{
    [TestClass]
    public class BackgroundTypeValuesTests
    {
        [TestMethod]
        public void BackgroundTypeValues_ParseToAsposeEnum_AllValuesParse()
        {
            #region Assert

            BackgroundType e;
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.DiagonalCrosshatch, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.DiagonalStripe, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.Gray12, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.Gray25, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.Gray50, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.Gray6, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.Gray75, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.HorizontalStripe, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.None, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ReverseDiagonalStripe, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.Solid, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ThickDiagonalCrosshatch, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ThinDiagonalCrosshatch, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ThinDiagonalStripe, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ThinHorizontalCrosshatch, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ThinHorizontalStripe, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ThinReverseDiagonalStripe, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.ThinVerticalStripe, out e));
            Assert.IsTrue(Enum.TryParse(BackgroundTypeValues.VerticalStripe, out e));

            #endregion
        }
    }
}