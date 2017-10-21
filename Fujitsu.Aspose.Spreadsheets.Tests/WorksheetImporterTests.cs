using System;
using System.Diagnostics.CodeAnalysis;
using Aspose.Cells;
using Fujitsu.Aspose.Spreadsheets.Tests.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Fujitsu.Aspose.Spreadsheets.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\Order.xlsx")]
    [DeploymentItem("Fujitsu.Aspose.Spreadsheets.Tests\\InvalidOrder.xlsx")]
    public class WorksheetImporterTests
    {
        private Workbook _workbook;
        private WorksheetImporter _worksheetImporter;

        private const int OrderId = 1345;
        private const string OrderRef = "A4523";
        private readonly DateTime OrderDate = new DateTime(2013, 2, 1);
        private const decimal OrderTotal = 14.86M;

        private const string ItemNo = "W154";
        private const decimal LineTotal = 6.9M;



        [TestInitialize]

        public void Initialize()
        {
            _worksheetImporter = new WorksheetImporter();
        }

        [TestMethod]
        [ExpectedException(typeof(MissingAttributeException<InvalidOrder, WorksheetItemAttribute>))]
        public void Import_WithTypeThatDoesntHaveTheCorrectAttributeThrowsException()
        {
            #region Arrange

            _workbook = new Workbook("Order.xlsx");

            #endregion

            #region Act

            _worksheetImporter.Import<InvalidOrder>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            #endregion
        }

        [TestMethod]
        [ExpectedException(typeof(MissingValueAttributeException<OrderWithNoValueAttributes, WorksheetItemValueAttribute>))]
        public void Import_WithTypeThatDoesntHaveTheCorrectValueAttributeThrowsException()
        {
            #region Arrange

            _workbook = new Workbook("Order.xlsx");

            #endregion

            #region Act

            _worksheetImporter.Import<OrderWithNoValueAttributes>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            #endregion
        }


        [TestMethod]
        public void Import_WithCorrectTypeReturnsCorrectResultType()
        {
            #region Arrange

            var expectedType = typeof(Order);

            _workbook = new Workbook("Order.xlsx");

            #endregion

            #region Act

            var result = _worksheetImporter.Import<Order>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.IsNotNull(result.Result);
            Assert.IsInstanceOfType(result.Result, expectedType);

            #endregion
        }

        [TestMethod]
        public void Import_WithCorrectTypeReturnsCorrectResultValues()
        {
            #region Arrange

            var expectedType = typeof(Order);

            _workbook = new Workbook("Order.xlsx");

            #endregion

            #region Act

            var result = _worksheetImporter.Import<Order>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.IsNotNull(result.Result);
            Assert.AreEqual(OrderTotal, result.Result.OrderTotal);
            Assert.AreEqual(OrderId, result.Result.OrderId);
            Assert.AreEqual(OrderDate, result.Result.OrderDate);
            Assert.AreEqual(OrderRef, result.Result.OrderRef);

            #endregion
        }


        [TestMethod]

        public void Import_WithMissingValuesReturnsCorrectResultValues()
        {
            #region Arrange

            var expectedType = typeof(Order);

            _workbook = new Workbook("InvalidOrder.xlsx");

            #endregion

            #region Act

            var result = _worksheetImporter.Import<Order>(_workbook.Worksheets[0]);

            #endregion

            #region Assert

            Assert.IsNotNull(result.Result);
            Assert.AreEqual(3, result.ValidationResults.Count);

            #endregion
        }

        [TestMethod]
        public void Import_ImportsAFullComplexObjectUsingBothImporters()
        {
            #region Arrange

            _workbook = new Workbook("Order.xlsx");
            var listImporter = new WorksheetListImporter();
            var worksheet = _workbook.Worksheets[0];
            #endregion

            #region Act

            // get the order part
            var result = _worksheetImporter.Import<Order>(worksheet);
            var order = result.Result;

            // now get the line items
            var listResult = listImporter.Import<OrderItem>(worksheet);
            order.OrderItems = listResult.Results;

            #endregion

            #region Assert

            Assert.IsNotNull(order);
            Assert.AreEqual(2, order.OrderItems.Count);

            // check some values of the line items
            Assert.AreEqual(ItemNo, order.OrderItems[0].ItemNo);
            Assert.AreEqual(LineTotal, order.OrderItems[0].LineTotal);

            #endregion
        }

        [TestMethod]
        public void WorkbookExtension_ExportToHtml_WithInvalidTabReturnsNull()
        {
            #region Arrange

            _workbook = new Workbook("Order.xlsx");

            #endregion

            #region Act

            // get the order part
            var result = _workbook.ExportToHtml("wibble");

            #endregion

            #region Assert

            Assert.IsNull(result);

            #endregion
        }

    }
}
