using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetItemValueAttribute : Attribute
    {
        public String CellReference { get; private set; }

        public bool IsFormattedValue { get; set; }

        public string FormatString { get; set; }

        public string PropertyArgumentNames { get; set; }

        public WorksheetItemValueAttribute(String cellReference)
        {
            CellReference = cellReference;
        }
    }
}
