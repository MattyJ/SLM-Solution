using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets.Types
{
    public class NamedValue<T>
    {
        public String Name { get; set; }

        public T Value { get; set; }
    }
}
