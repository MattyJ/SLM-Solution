using System;
using Fujitsu.Aspose.Spreadsheets.Dependency;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    /// <summary>
    /// Insert an additional row into a spreadsheet once the data has been processed. If multiple attributes
    /// for the same row are provided, they will be merged together.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class WorksheetRowAttribute : StyleAttribute
    {
        public WorksheetRowAttribute()
        {
            this.Row = int.MinValue;
            this.StartColumn = 0;
            this.EndColumn = int.MinValue;
        }

        /// <summary>
        /// The position where the row will be inserted. This is zero based. If left empty, the
        /// row will be placed at the end.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// The start column where the value will be inserted. This is zero based and will default
        /// to 0.
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// The column to end applying the style to. Is zero based and will default to maximum number of columns.
        /// If this is more than 1, then the columns will be merged.
        /// </summary>
        public int EndColumn { get; set; }

        /// <summary>
        /// The value to be placed into a cell on the row.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The formula to be placed into a cell on the row.
        /// </summary>
        public string ValueFormula { get; set; }

        /// <summary>
        /// The name of a <see cref="ICellValue"/>
        /// hook to be resolved and the resultant value placed into the cell.
        /// </summary>
        public string ValueExternal { get; set; }
    }
}
