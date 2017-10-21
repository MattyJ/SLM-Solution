using System;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    /// <summary>
    /// An attribute used to add style to a column. These will be overrideen by any Worksheet level styles.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class WorksheetColumnHeaderStyleAttribute : WorksheetColumnStyleAttribute
    {
    }
}