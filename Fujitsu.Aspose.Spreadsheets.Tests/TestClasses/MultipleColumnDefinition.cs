using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.Aspose.Spreadsheets.Types;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(1, 0)]
    public class MultipleColumnDefinition
    {
        [WorksheetRowItemValue("Install Date")]
        public String InstallDate { get; set; }

        [WorksheetRowItemValue("COMMENTS / NOTES")]
        public string Description { get; set; }

        [WorksheetRowMultipleItemValueAttribute("C-")]
        public List<NamedValue<int>> CColumns { get; set; }
    }
}
