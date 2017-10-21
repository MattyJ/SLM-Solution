using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Fujitsu.Aspose.Spreadsheets.Tests
{
    [TestClass]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\Software.xlsx")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\SoftwareLinks.xlsx")]
    [ExcludeFromCodeCoverage]
    public class WorksheetListGeneratorTests
    {
        private Workbook _workbook;
        private WorksheetListGenerator _worksheetListExporter;

        [TestInitialize]
        public void Initialize()
        {
            _worksheetListExporter = new WorksheetListGenerator();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "worksheet")]
        public void Generate_NullWorksheet_ThrowsException()
        {
            #region Arrange

            var softwareItems = new List<SoftwareItem>();

            #endregion

            #region Act

            _worksheetListExporter.Generate(null, softwareItems);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        public void Generate_UpdatesSoftwareFieldsCorrectly()
        {
            #region Arrange

            _workbook = new Workbook("Software.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            var softwareItems = new List<SoftwareItem>
                {
                    new SoftwareItem {Cost = 12.88M, Id = "Soft1", Quantity = 3, SoftwareName = "Excel"},
                    new SoftwareItem {Cost = 22.33M, Id = "Soft2", Quantity = 9, SoftwareName = "Word"},
                    new SoftwareItem {Cost = 75.6M, Id = "Soft3", Quantity = 24, SoftwareName = "Publisher"},
                };

            var hardwareItems = new List<HardwareItem>
                {
                    new HardwareItem() {Cost = 12.88M, Id = "Soft1", Quantity = 3, HardwareName = "Excel"},
                    new HardwareItem() {Cost = 22.33M, Id = "Soft2", Quantity = 9, HardwareName = "Word"},
                    new HardwareItem() {Cost = 75.6M, Id = "Soft3", Quantity = 24, HardwareName = "Publisher"},
                };

            #endregion

            #region Act

            _worksheetListExporter.Generate(worksheet, softwareItems);

            var hardwareRow = worksheet.FindCellWithValue("Hardware ID").Row;

            _worksheetListExporter.Generate(worksheet, hardwareItems, hardwareRow, hardwareRow + 1);


            #endregion

            #region Assert

            var row10Blank = worksheet.Cells.Rows[10].IsBlank;
            var row11Blank = worksheet.Cells.Rows[11].IsBlank;

            Assert.IsFalse(row10Blank);
            Assert.IsTrue(row11Blank);

            // now assert a few values for the 2nd row
            var row = worksheet.Cells.Rows[9];

            Assert.AreEqual(softwareItems[1].Cost.ToString(CultureInfo.InvariantCulture), row[5].Value.ToString());
            Assert.AreEqual(softwareItems[1].Id, row[1].Value);
            Assert.AreEqual(softwareItems[1].SoftwareName, row[3].Value);
            Assert.AreEqual(softwareItems[1].Quantity, row[4].Value);
            Assert.AreEqual(softwareItems[1].TotalCost.ToString(CultureInfo.InvariantCulture), row[6].Value.ToString());

            //_workbook.Save("C:\\Temp\\Software.xlsx");

            #endregion
        }

        [TestMethod]
        public void Generate_UpdatesSoftwareHyperlinkFieldsCorrectly()
        {
            #region Arrange

            _workbook = new Workbook("SoftwareLinks.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            var softwareItems = new List<SoftwareItemLink>
                {
                    new SoftwareItemLink {Cost = 12.88M, Id = "21", Quantity = 3, SoftwareName = "Excel"},
                    new SoftwareItemLink {Cost = 22.33M, Id = "22", Quantity = 9, SoftwareName = "Word"},
                    new SoftwareItemLink {Cost = 75.6M, Id = "23", Quantity = 24, SoftwareName = "Publisher"},
                };

            var hardwareItems = new List<HardwareItem>
                {
                    new HardwareItem() {Cost = 12.88M, Id = "Soft1", Quantity = 3, HardwareName = "Excel"},
                    new HardwareItem() {Cost = 22.33M, Id = "Soft2", Quantity = 9, HardwareName = "Word"},
                    new HardwareItem() {Cost = 75.6M, Id = "Soft3", Quantity = 24, HardwareName = "Publisher"},
                };

            #endregion

            #region Act

            _worksheetListExporter.Generate(worksheet, softwareItems);

            var hardwareRow = worksheet.FindCellWithValue("Hardware ID").Row;

            _worksheetListExporter.Generate(worksheet, hardwareItems, hardwareRow, hardwareRow + 1);


            #endregion

            #region Assert

            var row10Blank = worksheet.Cells.Rows[10].IsBlank;
            var row11Blank = worksheet.Cells.Rows[11].IsBlank;

            Assert.IsFalse(row10Blank);
            Assert.IsTrue(row11Blank);

            // now assert a few values for the 2nd row
            var row = worksheet.Cells.Rows[9];

            Assert.AreEqual(softwareItems[1].Cost.ToString(CultureInfo.InvariantCulture), row[5].Value.ToString());
            //Assert.AreEqual(softwareItems[1].Id, row[1].Value);
            Assert.AreEqual(softwareItems[1].SoftwareName, row[3].Value);
            Assert.AreEqual(softwareItems[1].Quantity, row[4].Value);
            Assert.AreEqual(softwareItems[1].TotalCost.ToString(CultureInfo.InvariantCulture), row[6].Value.ToString());

            _workbook.CalculateFormula();

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(MissingAttributeException<SoftwareItemNoWorksheetItem, WorksheetRowItemAttribute>))]
        public void Generate_InvalidTemplate_ThrowsException()
        {
            #region Arrange

            _workbook = new Workbook("Software.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            #endregion

            #region Act

            _worksheetListExporter.Generate(worksheet, new List<SoftwareItemNoWorksheetItem>());

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(MissingValueAttributeException<SoftwareItemNoValueAttributes, WorksheetRowItemValueAttribute>))]
        public void Generate_NoItemValueAttribute_ThrowsException()
        {
            #region Arrange

            _workbook = new Workbook("Software.xlsx");
            var worksheet = _workbook.Worksheets["Software"];

            #endregion

            #region Act

            _worksheetListExporter.Generate(worksheet, new List<SoftwareItemNoValueAttributes>());

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        public void Generate_NullModel_DoesNotThrowException()
        {
            #region Arrange

            _workbook = new Workbook("Software.xlsx");
            var worksheet = _workbook.Worksheets["Software"];
            List<SoftwareItem> model = null;

            #endregion

            #region Act

            _worksheetListExporter.Generate(worksheet, model);

            #endregion

            #region Assert

            #endregion
        }

    }
}
