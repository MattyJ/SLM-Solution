using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetItem]
    public class SoftwareSummary
    {
        [WorksheetItemValue("C3")]
        public String DesignTitle { get; set; }

        [WorksheetItemValue("C5")]
        public DateTime LastUpdated { get; set; }

        [WorkbookCustomProperty("Department")]
        public string Department { get; set; }

        [WorkbookBuiltInProperty("Company")]
        public string Company { get; set; }

        [WorkbookNamedRange("RangeSet")]
        public string RangeSet { get; set; }
    }
}
