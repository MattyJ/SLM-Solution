using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Constants;
using Fujitsu.Aspose.Spreadsheets.Tests.TestClasses;
using Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;

namespace Fujitsu.Aspose.Spreadsheets.Tests
{
    [TestClass]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\Software.xlsx")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\SoftwareLinks.xlsx")]
    [ExcludeFromCodeCoverage]
    public class WorksheetGeneratorTests
    {
        private Workbook _workbook;
        private WorksheetGenerator _worksheetExporter;
        private List<GenericReportDefinition> _reportDefinitionData;
        private List<AnotherReportDefinition> _moreReportDefinitionData;

        [TestInitialize]
        public void Initialize()
        {
            _worksheetExporter = new WorksheetGenerator();
            _reportDefinitionData = new List<GenericReportDefinition>()
            {
                new GenericReportDefinition()
                {
                    FirstName = "Walter",
                    LastName = "White",
                    OnList = false,
                    Salary = 23985.56m
                },
                new GenericReportDefinition()
                {
                    FirstName = "Jessie",
                    LastName = "Pinkman",
                    OnList = false,
                    Salary = 1233985.56m,
                    VoluntaryContribution = 923.93m
                },
                new GenericReportDefinition()
                {
                    FirstName = "Hank",
                    LastName = "Schrader",
                    OnList = false,
                    Salary = 53585.56m
                }
            };

            _moreReportDefinitionData = new List<AnotherReportDefinition>()
            {
                new AnotherReportDefinition()
                {
                    FirstName = "Walter",
                    LastName = "White",
                    OnList = false,
                    Salary = 23985.56m
                },
                new AnotherReportDefinition()
                {
                    FirstName = "Jessie",
                    LastName = "Pinkman",
                    OnList = false,
                    Salary = 1233985.56m,
                    VoluntaryContribution = 923.93m
                },
                new AnotherReportDefinition()
                {
                    FirstName = "Hank",
                    LastName = "Schrader",
                    OnList = false,
                    Salary = 53585.56m
                }
            };
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "worksheet")]
        public void Generate_NullWorksheet_ThrowsException()
        {
            #region Arrange

            var softwareSummary = new SoftwareSummary();

            #endregion

            #region Act

            _worksheetExporter.Generate(null, softwareSummary);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        public void Generate_UpdatesProjectFieldsCorrectly()
        {
            #region Arrange

            _workbook = new Workbook("Software.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            var softwareSummary = new SoftwareSummary
            {
                DesignTitle = "My Title",
                LastUpdated = DateTime.Now.AddDays(-3),
                Company = "Fujitsu - BAS",
                Department = "MAF Team",
                RangeSet = "My Range Value"
            };

            #endregion

            #region Act

            _worksheetExporter.Generate(worksheet, softwareSummary);

            #endregion

            #region Assert

            var titleValue = worksheet.Cells["C3"].Value;
            var lastUpdatedValue = worksheet.Cells["C5"].DateTimeValue;

            Assert.AreEqual(softwareSummary.DesignTitle, titleValue);
            Assert.AreEqual(softwareSummary.LastUpdated.ToString(CultureInfo.InvariantCulture), lastUpdatedValue.ToString(CultureInfo.InvariantCulture));

            Assert.AreEqual(softwareSummary.Company, _workbook.Worksheets.BuiltInDocumentProperties["Company"].Value);
            Assert.AreEqual(softwareSummary.Department, _workbook.Worksheets.CustomDocumentProperties["Department"].Value);
            Assert.AreEqual(softwareSummary.RangeSet, _workbook.Worksheets.GetRangeByName("RangeSet").Value);

            #endregion
        }

        [TestMethod]
        public void Generate_UpdatesProjectFieldLinksCorrectly()
        {
            #region Arrange

            _workbook = new Workbook("SoftwareLinks.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            var softwareSummary = new SoftwareSummaryLinks
            {
                DesignTitle = "My Title",
                LastUpdated = DateTime.Now.AddDays(-3),
                Company = "Fujitsu - BAS",
                Department = "1",
                RangeSet = "My Range Value",
                Id = 22
            };

            #endregion

            #region Act

            _worksheetExporter.Generate(worksheet, softwareSummary);

            #endregion

            #region Assert

            var titleValue = worksheet.Cells["C3"].Value;
            var lastUpdatedValue = worksheet.Cells["C5"].DateTimeValue;

            //Assert.AreEqual(softwareSummary.DesignTitle, titleValue);
            Assert.AreEqual(softwareSummary.LastUpdated.ToString(CultureInfo.InvariantCulture), lastUpdatedValue.ToString(CultureInfo.InvariantCulture));

            Assert.AreEqual(softwareSummary.Company, _workbook.Worksheets.BuiltInDocumentProperties["Company"].Value);
            Assert.AreEqual(softwareSummary.Department, _workbook.Worksheets.CustomDocumentProperties["Department"].Value);
            Assert.AreEqual(softwareSummary.RangeSet, _workbook.Worksheets.GetRangeByName("RangeSet").Value);

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(MissingAttributeException<SoftwareSummaryNoItemAttribute, WorksheetItemAttribute>))]
        public void Generate_ModelWithNoWorksheetItemAttribute_ThrowsException()
        {
            #region Arrange

            _workbook = new Workbook("Software.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            var softwareSummary = new SoftwareSummaryNoItemAttribute
            {
                DesignTitle = "My Title",
                LastUpdated = DateTime.Now.AddDays(-3)
            };

            #endregion

            #region Act

            _worksheetExporter.Generate(worksheet, softwareSummary);

            #endregion

            #region Assert


            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(MissingValueAttributeException<SoftwareSummaryNoItemValueAttributes, WorksheetItemValueAttribute>))]
        public void Generate_ModelWithNoWorksheetItemValueAttribute_ThrowsException()
        {
            #region Arrange

            _workbook = new Workbook("Software.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            var softwareSummary = new SoftwareSummaryNoItemValueAttributes
            {
                DesignTitle = "My Title",
                LastUpdated = DateTime.Now.AddDays(-3)
            };

            #endregion

            #region Act

            _worksheetExporter.Generate(worksheet, softwareSummary);

            #endregion

            #region Assert


            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetPopulatedWithCorrectData()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _reportDefinitionData);

            #endregion

            #region Assert

            Assert.AreEqual("Last Name", worksheet.Cells["B1"].Value);
            Assert.AreEqual(null, worksheet.Cells["D2"].Value);
            Assert.AreEqual(923.93, worksheet.Cells["D3"].Value);

            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetPropertiesSet()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _reportDefinitionData);

            #endregion

            #region Assert

            Assert.AreEqual("Cars", worksheet.Name);
            Assert.AreEqual(ColorTranslator.FromHtml(ColorValues.Black).ToKnownColor(), worksheet.TabColor.ToKnownColor());
            Assert.AreEqual(25, worksheet.Cells.StandardWidth);

            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetStylesSet()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _reportDefinitionData);

            #endregion

            #region Assert

            var styledCells = new[] { "A1", "A2", "B1", "B2" };
            var notStyledCells = new[] { "A3", "B3", "C3", "C2", "C1" };

            foreach (var cellLocation in styledCells)
            {
                var cell = worksheet.Cells[cellLocation];
                var style = cell.GetStyle();
                Assert.AreEqual(0, style.BackgroundColor.B);
                Assert.AreEqual(128, style.BackgroundColor.G);
                Assert.AreEqual(203, style.ForegroundColor.B);
                Assert.AreEqual(192, style.ForegroundColor.G);
                Assert.AreEqual(TextAlignmentType.Center, style.HorizontalAlignment);
                Assert.AreEqual(true, style.IsTextWrapped);
            }

            foreach (var cellLocation in notStyledCells)
            {
                var cell = worksheet.Cells[cellLocation];
                var style = cell.GetStyle();
                Assert.AreNotEqual(128, style.BackgroundColor.G);
                Assert.AreNotEqual(255, style.ForegroundColor.B);
                Assert.AreNotEqual(255, style.ForegroundColor.G);
                Assert.AreNotEqual(TextAlignmentType.Center, style.HorizontalAlignment);
                Assert.AreNotEqual(true, style.IsTextWrapped);
            }

            styledCells = new[] { "C1", "C2", "C3", };
            notStyledCells = new[] { "D1", "D2", "D3", };

            foreach (var cellLocation in styledCells)
            {
                var cell = worksheet.Cells[cellLocation];
                var style = cell.GetStyle();
                Assert.AreEqual("£#,##0.00", style.Custom);
            }

            foreach (var cellLocation in notStyledCells)
            {
                var cell = worksheet.Cells[cellLocation];
                var style = cell.GetStyle();
                Assert.AreNotEqual("£#,##0.00", style.Custom);
            }

            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetColumnStylesSet()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _moreReportDefinitionData);

            #endregion

            #region Assert

            var styledCells = new[] { "C1", "C2", "C3" };
            var notStyledCells = new[] { "B1", "B2", "B3", "D1", "D2", "D3" };

            foreach (var cellLocation in styledCells)
            {
                var cell = worksheet.Cells[cellLocation];
                var style = cell.GetStyle();
                Assert.AreEqual("£#,##0.00", style.Custom);
            }

            foreach (var cellLocation in notStyledCells)
            {
                var cell = worksheet.Cells[cellLocation];
                var style = cell.GetStyle();
                Assert.AreNotEqual("£#,##0.00", style.Custom);
            }

            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetColumnPropertiesSet()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _reportDefinitionData);

            #endregion

            #region Assert

            Assert.AreEqual(29, worksheet.Cells.GetColumnWidth(2), 1);

            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetFreezePropertiesSet()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _reportDefinitionData);

            #endregion

            #region Assert

            var row = 0;
            var column = 0;
            var freezedRows = 0;
            var freezedColumns = 0;
            worksheet.GetFreezedPanes(out row, out column, out freezedRows, out freezedColumns);

            Assert.AreEqual(1, row);
            Assert.AreEqual(0, column);
            Assert.AreEqual(1, freezedRows);

            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetFreezePropertiesSet_DefaultsCorrectlySet()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _moreReportDefinitionData);

            #endregion

            #region Assert

            Assert.AreEqual("A3:E3", worksheet.AutoFilter.Range);

            #endregion
        }

        [TestMethod]
        public void Generate_FromData_WorksheetFreezePropertiesSet_ActualValuesApplied()
        {
            #region Arrange

            var workbook = new Workbook();
            workbook.Worksheets.Add();

            var worksheet = workbook.Worksheets[0];

            #endregion

            #region Act

            _worksheetExporter.GenerateFromDefinition(worksheet, _reportDefinitionData);

            #endregion

            #region Assert

            Assert.AreEqual("A1:D1", worksheet.AutoFilter.Range);

            #endregion
        }
    }
}