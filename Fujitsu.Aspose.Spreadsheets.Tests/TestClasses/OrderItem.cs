namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(8, 7, StopAtFirstEmptyRow = true)]
    public class OrderItem
    {
        [WorksheetRowItemValue("Item No")]
        public string ItemNo { get; set; }

        [WorksheetRowItemValue("Item Description")]
        public string Description { get; set; }

        [WorksheetRowItemValue("Quantity")]
        public int Quantity { get; set; }

        [WorksheetRowItemValue("Unit Cost")]
        public decimal UnitCost { get; set; }

        [WorksheetRowItemValue("Line Total")]
        public decimal LineTotal { get; set; }

    }
}