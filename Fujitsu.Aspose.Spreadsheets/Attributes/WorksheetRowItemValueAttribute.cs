using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetRowItemValueAttribute : Attribute
    {
        public String ColumnName { get; private set; }

        public Int32 ColumnIndex { get; set; }

        public bool IsFormattedValue { get; set; }

        public string FormatString { get; set; }

        public string PropertyArgumentNames { get; set; }

        public WorksheetRowItemValueAttribute(String columnName = null)
        {
            ColumnName = columnName;
            ColumnIndex = -1;
        }
    }
}
