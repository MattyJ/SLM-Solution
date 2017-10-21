using System;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [ExcludeFromCodeCoverage]
    public class SoftwareItemNoWorksheetItem
    {
        [WorksheetRowItemValue("Software Id")]
        public String Id { get; set; }

        [WorksheetRowItemValue("Name")]
        public String SoftwareName { get; set; }

        [WorksheetRowItemValue("Cost")]
        public decimal Cost { get; set; }

        [WorksheetRowItemValue("Quantity")]
        public int Quantity { get; set; }

        [WorksheetRowItemValue("Total")]
        public decimal TotalCost { get { return Cost * Quantity; } }

    }
}