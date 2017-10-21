using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorkbookBuiltInPropertyAttribute : Attribute
    {
        public String PropertyName { get; private set; }

        public WorkbookBuiltInPropertyAttribute(String propertyName)
        {
            PropertyName = propertyName;
        }
    }
}
