using System;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    public class SoftwareSummaryNoItemAttribute
    {
        [WorksheetItemValue("C3")]
        public String DesignTitle { get; set; }

        [WorksheetItemValue("C5")]
        public DateTime LastUpdated { get; set; }
    }
}