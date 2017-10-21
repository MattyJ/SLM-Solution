using System;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    /// <summary>
    /// An attribute used to place a filter on the worksheet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WorksheetFilterAttribute : Attribute
    {
        public WorksheetFilterAttribute()
        {
            this.StartColumn = int.MinValue;
            this.EndColumn = int.MinValue;
        }

        /// <summary>
        /// Is zero-based.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// If not supplied, this will default to 0. Is zero-based.
        /// </summary>
        public int StartColumn { get; set; }

        /// <summary>
        /// If not supplied, this will default to the number of max columns. Is zero-based.
        /// </summary>
        public int EndColumn { get; set; }
    }
}