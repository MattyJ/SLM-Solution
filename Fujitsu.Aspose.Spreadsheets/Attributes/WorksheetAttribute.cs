using System;
using System.Drawing;

namespace Fujitsu.Aspose.Spreadsheets.Attributes
{
    /// <summary>
    /// An attribute used to describe a generically generated worksheet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class WorksheetAttribute : Attribute
    {
        public WorksheetAttribute()
        {
            this.StandardWidth = int.MinValue;
        }

        /// <summary>
        /// The name of the tab for this worksheet.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The color of the tab for this worksheet. Must be supplied in hex format e.g. #0000FF. The class
        /// <see cref="Fujitsu.Aspose.Spreadsheets.Constants.ColorValues"/> is available for common
        /// colors.
        /// </summary>
        public string TabColor { get; set; }

        /// <summary>
        /// The standard width of the columns on the spreadsheet.
        /// </summary>
        public int StandardWidth { get; set; }
    }
}