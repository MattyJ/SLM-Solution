using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(8, 7, DuplicateFirstRowOnGenerate = true)]
    public class SoftwareItem
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
