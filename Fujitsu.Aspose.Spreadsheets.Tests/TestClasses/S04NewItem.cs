using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.Aspose.Spreadsheets.Types;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(5, 4)]
    public class S04NewItem
    {
        [WorksheetRowItemValue("Data Centre")]
        public String DataCentre { get; set; }

        [WorksheetRowItemValue("e-zone")]
        public String Ezone { get; set; }

        [WorksheetRowItemValue("Install Date")]
        public String InstallDate { get; set; }

        [WorksheetRowMultipleItemValue("C.")]
        public List<NamedValue<double>> CColumns { get; set; }
    }
}
