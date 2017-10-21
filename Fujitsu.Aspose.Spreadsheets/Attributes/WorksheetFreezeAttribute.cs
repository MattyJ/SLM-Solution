using System;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    /// <summary>
    /// An attribute used to freeze a worksheet at a particular cell.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WorksheetFreezeAttribute : Attribute
    {
        public WorksheetFreezeAttribute()
        {
            this.FreezedColumns = int.MinValue;
            this.FreezedRows = int.MinValue;
        }

        /// <summary>
        /// Is zero-based.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Is zero-based.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// If not supplied, this will be set to the number of max rows.
        /// </summary>
        public int FreezedRows { get; set; }

        /// <summary>
        /// If not supplied, this will be set to the number of max columns.
        /// </summary>
        public int FreezedColumns { get; set; }
    }
}