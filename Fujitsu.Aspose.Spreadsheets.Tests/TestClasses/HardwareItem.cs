using System;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(11, 10, DuplicateFirstRowOnGenerate = true)]
    public class HardwareItem
    {
        [WorksheetRowItemValue("Hardware Id")]
        public String Id { get; set; }

        [WorksheetRowItemValue("Name")]
        public String HardwareName { get; set; }

        [WorksheetRowItemValue("Cost")]
        public decimal Cost { get; set; }

        [WorksheetRowItemValue("Quantity")]
        public int Quantity { get; set; }

        [WorksheetRowItemValue("Total")]
        public decimal TotalCost { get { return Cost * Quantity; } }

    }
}