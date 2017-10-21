using System;
using System.Net;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(8, 7, DuplicateFirstRowOnGenerate = true)]
    public class SoftwareItemLink
    {
        [WorksheetRowItemValue("Software Id", IsFormattedValue = true, FormatString =  "=HYPERLINK(CONCATENATE(\"http://europemuk654\",\"/Project/Details/\",\"{0}\"),\"{1}\")"
            , PropertyArgumentNames = "Id;SoftwareName")]
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