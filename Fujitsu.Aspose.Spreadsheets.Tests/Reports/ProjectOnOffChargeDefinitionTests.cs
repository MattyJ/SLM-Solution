using System.Collections.Generic;
using System.Drawing;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Constants;
using Fujitsu.Aspose.Spreadsheets.Dependency;
using Fujitsu.Aspose.Spreadsheets.Tests.Dependency;
using Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests.Reports
{
    [TestClass]
    public class ProjectOnOffChargeDefinitionTests
    {
        private List<ProjectOnOffChargeDefinition> _projectOnOffChargeDefinitions;
        private Workbook _workbook;
        private Worksheet _worksheet;

        [TestInitialize]
        public void TestInitialize()
        {
            this._projectOnOffChargeDefinitions = new List<ProjectOnOffChargeDefinition>()
            {
                UnitTestHelper.GenerateRandomData<ProjectOnOffChargeDefinition>(),
                UnitTestHelper.GenerateRandomData<ProjectOnOffChargeDefinition>(),
                UnitTestHelper.GenerateRandomData<ProjectOnOffChargeDefinition>(),
                UnitTestHelper.GenerateRandomData<ProjectOnOffChargeDefinition>(),
                UnitTestHelper.GenerateRandomData<ProjectOnOffChargeDefinition>()
            };

            this._workbook = new Workbook();
            this._workbook.Worksheets.Add();
            this._worksheet = this._workbook.Worksheets[0];

            DependencyResolver.SetResolver(new ObjectFactory());
        }
            
        [TestMethod]
        public void ProjectOnOffChargeDefinition_GenerateReport_DevelopmentAid_OutputToFile()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            //this._workbook.Save(@"C:\Temp\ProjectOnOffCharge.xls", new XlsSaveOptions(SaveFormat.Excel97To2003));
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetStyle_HeightApplied_RowHasHeightApplied()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            Assert.AreEqual(60.8D, this._worksheet.Cells.Rows[3].Height);
            Assert.AreNotEqual(60.8D, this._worksheet.Cells.Rows[2].Height);
            Assert.AreNotEqual(60.8D, this._worksheet.Cells.Rows[4].Height);
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetStyle_MultipleWorksheetRowForSameRow_RowsAreMergedTogether()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            Assert.AreEqual("FY 2011 - 2012", this._worksheet.Cells["G2"].Value);
            Assert.AreEqual("FY 2012 - 2013", this._worksheet.Cells["S2"].Value);
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetStyle_WorksheetRowSpansMultipleColumns_CellsAreMergedTogether()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            Assert.IsTrue(this._worksheet.Cells["G2"].IsMerged);
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetStyle_WorksheetRowWithFormula_FormulaIsApplied()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            Assert.IsTrue(this._worksheet.Cells["B1"].IsFormula);
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetStyle_WorksheetRowWithValueExternal_InstanceIsInjectedAndExecuted()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            Assert.AreEqual("XXX", this._worksheet.Cells["A3"].Value);
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetStyle_WorksheetRowWithValueNoRow_AdditionalRowInsertedAtEnd()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            Assert.AreEqual("Extra", this._worksheet.Cells["A10"].Value);
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetColumnHeaderStyle_HeaderHasTopBorder_CheckBorderTypeAndColorSet()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            var border = this._worksheet.Cells["C4"].GetStyle().Borders[BorderType.TopBorder];

            Assert.AreEqual(CellBorderType.Thin, border.LineStyle);
            Assert.AreEqual(ColorTranslator.FromHtml(ColorValues.Black).ToKnownColor(), border.Color.ToKnownColor());
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetColumnHeaderStyle_HeaderHasBottomBorder_CheckBorderTypeAndColorSet()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            var border = this._worksheet.Cells["C4"].GetStyle().Borders[BorderType.BottomBorder];

            Assert.AreEqual(CellBorderType.Thin, border.LineStyle);
            Assert.AreEqual(ColorTranslator.FromHtml(ColorValues.Black).ToKnownColor(), border.Color.ToKnownColor());
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetColumnHeaderStyle_HeaderHasLeftBorder_CheckBorderTypeAndColorSet()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            var border = this._worksheet.Cells["C4"].GetStyle().Borders[BorderType.LeftBorder];

            Assert.AreEqual(CellBorderType.Thin, border.LineStyle);
            Assert.AreEqual(ColorTranslator.FromHtml(ColorValues.Black).ToKnownColor(), border.Color.ToKnownColor());
        }

        [TestMethod]
        public void ProjectOnOffChargeDefinition_WorksheetColumnHeaderStyle_HeaderHasRightBorder_CheckBorderTypeAndColorSet()
        {
            var generator = new WorksheetGenerator();
            generator.GenerateFromDefinition(this._worksheet, this._projectOnOffChargeDefinitions);

            var border = this._worksheet.Cells["C4"].GetStyle().Borders[BorderType.RightBorder];

            Assert.AreEqual(CellBorderType.Thin, border.LineStyle);
            Assert.AreEqual(ColorTranslator.FromHtml(ColorValues.Black).ToKnownColor(), border.Color.ToKnownColor());
        }
    }
}
