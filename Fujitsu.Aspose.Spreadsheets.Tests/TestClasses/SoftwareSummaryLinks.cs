using System;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetItem]
    public class SoftwareSummaryLinks
    {
        [WorksheetItemValue("C3", IsFormattedValue = true, FormatString = "=HYPERLINK(CONCATENATE(\"http://europemuk654\",\"/Project/Details/\",\"{0}\"),\"{1}\")", 
            PropertyArgumentNames = "Id;DesignTitle")]
        public String DesignTitle { get; set; }

        [WorksheetItemValue("C5")]
        public DateTime LastUpdated { get; set; }

        [WorkbookCustomProperty("Department")]
        public string Department { get; set; }

        [WorkbookBuiltInProperty("Company")]
        public string Company { get; set; }

        [WorkbookNamedRange("RangeSet")]
        public string RangeSet { get; set; }

        public int Id { get; set; }
    }
}