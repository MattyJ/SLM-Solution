using Fujitsu.Aspose.Spreadsheets.Attributes;
using Fujitsu.Aspose.Spreadsheets.Constants;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions
{
    [Worksheet(Name="Cars", TabColor = ColorValues.Black)]
    [WorksheetFilter(Row = 2)]
    public class AnotherReportDefinition
    {
        [WorksheetColumn(Ordinal = 1, Name="First Name")]
        public string FirstName { get; set; }
        [WorksheetColumn(Ordinal = 2, Name = "Last Name")]
        public string LastName { get; set; }
        [WorksheetColumn(Ordinal = 3, Name = "Salary", Width = 29)]
        [WorksheetColumnStyle(Custom = "£#,##0.00", NumberFormat = true)]
        public decimal Salary { get; set; }
        [WorksheetColumn(Ordinal = 4, Name = "Voluntary Contribution")]
        public decimal? VoluntaryContribution { get; set; }
        [WorksheetColumn(Ordinal = 5, Name = "On List")]
        public bool OnList { get; set; }
    }
}