using System;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetRowMultipleRegExItemValueAttribute : Attribute
    {
        public String Expression { get; private set; }

        public WorksheetRowMultipleRegExItemValueAttribute(String expression)
        {
            Expression = expression;
        }
    }
}