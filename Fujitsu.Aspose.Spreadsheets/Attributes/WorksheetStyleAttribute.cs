using System;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    /// <summary>
    /// An attribute used to add styles to cells.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WorksheetStyleAttribute : StyleAttribute
    {
        public WorksheetStyleAttribute()
        {
            this.EndColumn = int.MinValue;
            this.EndRow = int.MinValue;
            this.Height = double.MinValue;
        }

        /// <summary>
        /// The column to start applying the style to. Is zero based and will default to 0.
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// The column to end applying the style to. Is zero based and will default to maximum number of columns.
        /// </summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// The row to start applying the style to. Is zero based and will default to 0.
        /// </summary>
        public int StartRow { get; set; }

        /// <summary>
        /// The row to end applying the style to. Is zero based and will default to maximum number of rows.
        /// </summary>
        public int EndRow { get; set; }

        /// <summary>
        /// Set the height of this range - ultimately, this will be the entire row.
        /// </summary>
        public double Height { get; set; }
    }
}