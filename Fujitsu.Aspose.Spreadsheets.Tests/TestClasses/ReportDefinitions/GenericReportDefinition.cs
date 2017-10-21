using Fujitsu.Aspose.Spreadsheets.Attributes;
using Fujitsu.Aspose.Spreadsheets.Constants;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions
{
    [Worksheet(Name="Cars", TabColor = ColorValues.Black, StandardWidth = 25)]
    [WorksheetFreeze(Column = 0, Row = 1, FreezedRows = 1)]
    [WorksheetFilter(Row = 0, StartColumn = 0, EndColumn = 3)]
    [WorksheetStyle(StartColumn = 0, StartRow = 0, EndColumn = 1, EndRow = 1, BackgroundColor = ColorValues.Olive,
        ForegroundColor = ColorValues.Pink, HorizontalAlign = TextAlignmentTypeValues.Center, IsTextWrapped = true)]
    [WorksheetStyle(StartColumn = 2, StartRow = 0, EndColumn = 2, NumberFormat = true, Custom = "£#,##0.00")]
    public class GenericReportDefinition
    {
        [WorksheetColumn(Ordinal = 1, Name="First Name")]
        public string FirstName { get; set; }
        [WorksheetColumn(Ordinal = 2, Name = "Last Name")]
        public string LastName { get; set; }
        [WorksheetColumn(Ordinal = 3, Name = "Salary", Width = 29)]
        public decimal Salary { get; set; }
        [WorksheetColumn(Ordinal = 4, Name = "Voluntary Contribution")]
        public decimal? VoluntaryContribution { get; set; }
        [WorksheetColumn(Ordinal = 5, Name = "On List")]
        public bool OnList { get; set; }
    }
}