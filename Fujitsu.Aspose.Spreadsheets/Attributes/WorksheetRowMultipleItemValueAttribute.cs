using System;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetRowMultipleItemValueAttribute : Attribute
    {
        public String ColumnPrefix { get; private set; }

        public Int32 StartIndex { get; set; }

        public WorksheetRowMultipleItemValueAttribute(String columnPrefix)
        {
            ColumnPrefix = columnPrefix;
            StartIndex = -1;
        }
    }
}