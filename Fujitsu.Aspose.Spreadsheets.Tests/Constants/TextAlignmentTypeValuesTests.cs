using System;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Constants;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests.Constants
{
    [TestClass]
    public class TextAlignmentTypeValuesTests
    {
        [TestMethod]
        public void TextAlignmentTypeValues_ParseToAsposeEnum_AllValuesParse()
        {
            #region Assert

            TextAlignmentType e;
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.Bottom, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.Center, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.CenterAcross, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.Fill, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.General, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.Justify, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.Left, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.Right, out e));
            Assert.IsTrue(Enum.TryParse(TextAlignmentTypeValues.Top, out e));

            #endregion
        }
    }
}