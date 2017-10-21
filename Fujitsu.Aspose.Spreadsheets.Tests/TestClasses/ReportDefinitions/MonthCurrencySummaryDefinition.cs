using Fujitsu.Aspose.Spreadsheets.Attributes;
using Fujitsu.Aspose.Spreadsheets.Constants;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions
{
    [WorksheetFreeze(Row = 1, Column = 0, FreezedRows = 1)]
    [WorksheetFilter(Row = 0, StartColumn = 0)]
    [WorksheetStyle(StartColumn = 0, StartRow = 0, EndRow = 0, IsTextWrapped = true, HorizontalAlign = TextAlignmentTypeValues.Center,
        Pattern = BackgroundTypeValues.Solid, ForegroundColor = ColorValues.LightSkyBlue)]
    public abstract class MonthCurrencySummaryDefinition
    {
        [WorksheetColumn(Ordinal = 100, Name = "Apr-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        public decimal? Apr12 { get; set; }

        [WorksheetColumn(Ordinal = 101, Name = "May-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? May12 { get; set; }

        [WorksheetColumn(Ordinal = 102, Name = "Jun-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jun12 { get; set; }

        [WorksheetColumn(Ordinal = 103, Name = "Jul-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jul12 { get; set; }

        [WorksheetColumn(Ordinal = 104, Name = "Aug-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Aug12 { get; set; }

        [WorksheetColumn(Ordinal = 105, Name = "Sep-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Sep12 { get; set; }

        [WorksheetColumn(Ordinal = 106, Name = "Oct-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Oct12 { get; set; }

        [WorksheetColumn(Ordinal = 107, Name = "Nov-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Nov12 { get; set; }

        [WorksheetColumn(Ordinal = 108, Name = "Dec-12")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Dec12 { get; set; }

        [WorksheetColumn(Ordinal = 109, Name = "Jan-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jan13 { get; set; }

        [WorksheetColumn(Ordinal = 110, Name = "Feb-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Feb13 { get; set; }

        [WorksheetColumn(Ordinal = 111, Name = "Mar-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Mar13 { get; set; }

        [WorksheetColumn(Ordinal = 112, Name = "Apr-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Apr13 { get; set; }

        [WorksheetColumn(Ordinal = 113, Name = "May-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? May13 { get; set; }

        [WorksheetColumn(Ordinal = 114, Name = "Jun-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jun13 { get; set; }

        [WorksheetColumn(Ordinal = 115, Name = "Jul-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jul13 { get; set; }

        [WorksheetColumn(Ordinal = 116, Name = "Aug-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Aug13 { get; set; }

        [WorksheetColumn(Ordinal = 117, Name = "Sep-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Sep13 { get; set; }

        [WorksheetColumn(Ordinal = 118, Name = "Oct-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Oct13 { get; set; }

        [WorksheetColumn(Ordinal = 119, Name = "Nov-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Nov13 { get; set; }

        [WorksheetColumn(Ordinal = 120, Name = "Dec-13")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Dec13 { get; set; }

        [WorksheetColumn(Ordinal = 121, Name = "Jan-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jan14 { get; set; }

        [WorksheetColumn(Ordinal = 122, Name = "Feb-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Feb14 { get; set; }

        [WorksheetColumn(Ordinal = 123, Name = "Mar-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Mar14 { get; set; }

        [WorksheetColumn(Ordinal = 124, Name = "Apr-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Apr14 { get; set; }

        [WorksheetColumn(Ordinal = 125, Name = "May-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? May14 { get; set; }

        [WorksheetColumn(Ordinal = 126, Name = "Jun-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jun14 { get; set; }

        [WorksheetColumn(Ordinal = 127, Name = "Jul-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jul14 { get; set; }

        [WorksheetColumn(Ordinal = 128, Name = "Aug-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Aug14 { get; set; }

        [WorksheetColumn(Ordinal = 129, Name = "Sep-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Sep14 { get; set; }

        [WorksheetColumn(Ordinal = 130, Name = "Oct-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Oct14 { get; set; }

        [WorksheetColumn(Ordinal = 131, Name = "Nov-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Nov14 { get; set; }

        [WorksheetColumn(Ordinal = 132, Name = "Dec-14")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Dec14 { get; set; }

        [WorksheetColumn(Ordinal = 133, Name = "Jan-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jan15 { get; set; }

        [WorksheetColumn(Ordinal = 134, Name = "Feb-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Feb15 { get; set; }

        [WorksheetColumn(Ordinal = 135, Name = "Mar-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Mar15 { get; set; }

        [WorksheetColumn(Ordinal = 136, Name = "Apr-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Apr15 { get; set; }

        [WorksheetColumn(Ordinal = 137, Name = "May-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? May15 { get; set; }

        [WorksheetColumn(Ordinal = 138, Name = "Jun-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jun15 { get; set; }

        [WorksheetColumn(Ordinal = 139, Name = "Jul-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jul15 { get; set; }

        [WorksheetColumn(Ordinal = 140, Name = "Aug-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Aug15 { get; set; }

        [WorksheetColumn(Ordinal = 141, Name = "Sep-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Sep15 { get; set; }

        [WorksheetColumn(Ordinal = 142, Name = "Oct-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Oct15 { get; set; }

        [WorksheetColumn(Ordinal = 143, Name = "Nov-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Nov15 { get; set; }

        [WorksheetColumn(Ordinal = 144, Name = "Dec-15")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Dec15 { get; set; }

        [WorksheetColumn(Ordinal = 145, Name = "Jan-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jan16 { get; set; }

        [WorksheetColumn(Ordinal = 146, Name = "Feb-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Feb16 { get; set; }

        [WorksheetColumn(Ordinal = 147, Name = "Mar-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Mar16 { get; set; }

        [WorksheetColumn(Ordinal = 148, Name = "Apr-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Apr16 { get; set; }

        [WorksheetColumn(Ordinal = 149, Name = "May-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? May16 { get; set; }

        [WorksheetColumn(Ordinal = 150, Name = "Jun-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jun16 { get; set; }

        [WorksheetColumn(Ordinal = 151, Name = "Jul-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jul16 { get; set; }

        [WorksheetColumn(Ordinal = 152, Name = "Aug-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Aug16 { get; set; }

        [WorksheetColumn(Ordinal = 153, Name = "Sep-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Sep16 { get; set; }

        [WorksheetColumn(Ordinal = 154, Name = "Oct-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Oct16 { get; set; }

        [WorksheetColumn(Ordinal = 155, Name = "Nov-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Nov16 { get; set; }

        [WorksheetColumn(Ordinal = 156, Name = "Dec-16")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Dec16 { get; set; }

        [WorksheetColumn(Ordinal = 157, Name = "Jan-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jan17 { get; set; }

        [WorksheetColumn(Ordinal = 158, Name = "Feb-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Feb17 { get; set; }

        [WorksheetColumn(Ordinal = 159, Name = "Mar-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Mar17 { get; set; }

        [WorksheetColumn(Ordinal = 160, Name = "Apr-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Apr17 { get; set; }

        [WorksheetColumn(Ordinal = 161, Name = "May-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? May17 { get; set; }

        [WorksheetColumn(Ordinal = 162, Name = "Jun-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jun17 { get; set; }

        [WorksheetColumn(Ordinal = 163, Name = "Jul-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jul17 { get; set; }

        [WorksheetColumn(Ordinal = 164, Name = "Aug-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Aug17 { get; set; }

        [WorksheetColumn(Ordinal = 165, Name = "Sep-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Sep17 { get; set; }

        [WorksheetColumn(Ordinal = 166, Name = "Oct-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Oct17 { get; set; }

        [WorksheetColumn(Ordinal = 167, Name = "Nov-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Nov17 { get; set; }

        [WorksheetColumn(Ordinal = 168, Name = "Dec-17")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Dec17 { get; set; }

        [WorksheetColumn(Ordinal = 169, Name = "Jan-18")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Jan18 { get; set; }

        [WorksheetColumn(Ordinal = 170, Name = "Feb-18")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Feb18 { get; set; }

        [WorksheetColumn(Ordinal = 171, Name = "Mar-18")]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat, NumberFormat = true)]
        public decimal? Mar18 { get; set; }
    }
}