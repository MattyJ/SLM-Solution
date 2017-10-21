using System;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorkbookNamedRangeAttribute : Attribute
    {
        public String NamedRangeName { get; private set; }

        public WorkbookNamedRangeAttribute(String namedRangeName)
        {
            NamedRangeName = namedRangeName;
        }
    }
}