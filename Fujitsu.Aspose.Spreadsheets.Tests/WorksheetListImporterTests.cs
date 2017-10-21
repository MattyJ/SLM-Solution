using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\Users.xlsx")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\UsersWithBlankRow.xlsx")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\UsersWithNoTitles.xlsx")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\MultipleColumns.xlsx")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\IVR S04 New Install TRP640.xls")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\S04 PxQ Assets.xls")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\IT12345.xls")]
    public class WorksheetListImporterTests
    {
        private Workbook _workbook;
        private WorksheetListImporter _worksheetListImporter;

        [TestInitialize]
        public void Initialize()
        {
            _worksheetListImporter = new WorksheetListImporter();
        }


        [TestMethod]
        [ExpectedException(typeof(MissingAttributeException<InvalidUser, WorksheetRowItemAttribute>))]
        public void Import_WithTypeThatDoesntHaveTheCorrectAttributeThrowsException()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("Users.xlsx");

            #endregion

            #region Act

            _worksheetListImporter.Import<InvalidUser>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(MissingValueAttributeException<UserWithNoValueAttributes, WorksheetRowItemValueAttribute>))]
        public void Import_WithTypeThatDoesntHaveTheCorrectValueAttributeThrowsException()
        {
            #region Arrange

            _workbook = new Workbook("Users.xlsx");

            #endregion

            #region Act

            _worksheetListImporter.Import<UserWithNoValueAttributes>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        public void Import_WithCorrectTypeReturnsCorrectResultCount()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("Users.xlsx");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<User>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.AreEqual(3, result.Results.Count);
            Assert.AreEqual(5, result.ValidationResults.Count);

            #endregion
        }

        [TestMethod]
        public void Import_WithBlankRowReturnsCorrectResultCount()
        {
            #region Arrange

            _workbook = new Workbook("UsersWithBlankRow.xlsx");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<User>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.AreEqual(1, result.Results.Count);
            Assert.AreEqual(0, result.ValidationResults.Count);

            #endregion
        }

        [TestMethod]
        public void Import_WithValidBlankRowReturnsCorrectResultCount()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("UsersWithBlankRow.xlsx");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<UserAllowingBlankRows>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.AreEqual(3, result.Results.Count);
            Assert.AreEqual(5, result.ValidationResults.Count);

            #endregion
        }

        [TestMethod]
        public void Import_WithEmptyTitleRowSpecifiedReturnsEmptyList()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("UsersWithNoTitles.xlsx");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<User>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.AreEqual(0, result.Results.Count);

            #endregion
        }

        [TestMethod]
        public void Import_MultipleColumns()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("MultipleColumns.xlsx");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<MultipleColumnDefinition>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.AreEqual(5, result.Results.Count);

            #endregion
        }

        [TestMethod]
        public void Import_MultipleColumnsWithFormulas()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("IVR S04 New Install TRP640.xls");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<S04NewItem>(_workbook.Worksheets[1]);

            #endregion

            #region Assert

            Assert.AreEqual(30, result.Results.Count);

            #endregion
        }

        [TestMethod]
        public void Import_MultipleColumnsWithRegularExpression()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("S04 PxQ Assets.xls");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<S04PxQAssets>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.AreEqual(20, result.Results.Count);

            #endregion
        }

        [TestMethod]
        public void Import_MultipleColumnsWithRegularExpression_WithAnAlternativeTitleRow()
        {
            #region Arrange

            // ignore the txt extension.  this is a fudge to get over a deployment item "bug"
            _workbook = new Workbook("IT12345.xls");

            #endregion

            #region Act

            var result = _worksheetListImporter.Import<NewInstall>(_workbook.Worksheets[1]);

            #endregion

            #region Assert

            Assert.AreEqual(12, result.Results.Count);

            #endregion
        }

    }
}
