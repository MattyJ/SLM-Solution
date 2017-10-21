using System.Collections.Generic;
using Aspose.Cells;

namespace Fujitsu.Aspose.Spreadsheets.Types.Definition
{
    /// <summary>
    /// Used to hold the dynamically generated definition of a report.
    /// </summary>
    internal class ReportDefinition<T>
    {
        internal ReportDefinition()
        {
            this.Columns = new List<ColumnDefinition<T>>();
            this.Styles = new List<WorksheetStyleDefinition>();
            this.Rows = new List<RowDefinition>();
        }

        internal string Name { get; set; }
        internal string TabColor { get; set; }
        internal int? StandardWidth { get; set; }
        internal FreezeDefinition Freeze { get; set; }
        internal FilterDefinition Filter { get; set; }
        internal List<WorksheetStyleDefinition> Styles { get; set; } 
        internal List<ColumnDefinition<T>> Columns { get; private set; }
        internal List<RowDefinition> Rows { get; private set; } 
    }
}