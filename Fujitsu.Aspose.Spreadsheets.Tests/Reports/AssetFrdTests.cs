using System.Collections.Generic;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests.Reports
{
    [TestClass]
    public class AssetFrdTests
    {
        [TestMethod]
        public void AssetFrd_GenerateReport_Inheritance_ColumnOrdinalsRemainInOrder()
        {
            var data = new List<AssetFrdDefinition>()
            {
                UnitTestHelper.GenerateRandomData<AssetFrdDefinition>(),
                UnitTestHelper.GenerateRandomData<AssetFrdDefinition>(),
                UnitTestHelper.GenerateRandomData<AssetFrdDefinition>(),
                UnitTestHelper.GenerateRandomData<AssetFrdDefinition>(),
                UnitTestHelper.GenerateRandomData<AssetFrdDefinition>()
            };

            var workbook = new Workbook();
            workbook.Worksheets.Add();
            var worksheet = workbook.Worksheets[0];

            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(worksheet, data);

            var cell = worksheet.Cells["F2"];
            var style = cell.GetStyle();
            Assert.AreEqual(ReportFormatValues.CurrencyFormat, style.Custom);

            cell = worksheet.Cells["BY2"];
            style = cell.GetStyle();
            Assert.AreEqual(ReportFormatValues.CurrencyFormat, style.Custom);

            //workbook.Save(@"C:\Temp\AssetFrd.xls", new XlsSaveOptions(SaveFormat.Excel97To2003));
        }
    }
}
