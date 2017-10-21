using System;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    /// <summary>
    /// An attribute used to describe a column of a generically generated worksheet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class WorksheetColumnAttribute : Attribute
    {
        public WorksheetColumnAttribute()
        {
            this.Width = double.MinValue;
        }

        /// <summary>
        /// The position of the column within the worksheet horizintally. Numbering begins at 1.
        /// </summary>
        public int Ordinal { get; set; }

        /// <summary>
        /// The heading text of the column.
        /// </summary>
        public string Name { get; set; }

        public double Width { get; set; }
    }
}