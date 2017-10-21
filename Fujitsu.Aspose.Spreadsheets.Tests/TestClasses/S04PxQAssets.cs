using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.Aspose.Spreadsheets.Types;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(2, 1)]
    public class S04PxQAssets
    {
        [WorksheetRowItemValue("UID")]
        public String FaadId { get; set; }

        [WorksheetRowItemValue("Asset Tag")]
        public String AssetTag { get; set; }

        [WorksheetRowMultipleRegExItemValueAttribute("^[A-Z]{4}$")]
        public List<NamedValue<double>> CodeColumns { get; set; }
    }
}
