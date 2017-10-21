using System;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorkbookCustomPropertyAttribute : Attribute
    {
        public String PropertyName { get; private set; }

        public WorkbookCustomPropertyAttribute(String propertyName)
        {
            PropertyName = propertyName;
        }
    }
}