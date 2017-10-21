using Fujitsu.Aspose.Spreadsheets.Attributes;
using Fujitsu.Aspose.Spreadsheets.Constants;
using Fujitsu.Aspose.Spreadsheets.Tests.Dependency;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions
{
    [Worksheet(Name = "Project On Off Charge")]
    [WorksheetFreeze(Row = 1, Column = 0, FreezedRows = 1)]
    [WorksheetFilter(Row = 0, StartColumn = 0, EndColumn = 5)]
    [WorksheetStyle(StartColumn = 0, StartRow = 0, EndRow = 0, Height = 60.8D, IsTextWrapped = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 0, StartColumn = 0, EndColumn = 0, Value = "Current Month:", FontBold = true)]
    [WorksheetRow(Row = 0, StartColumn = 1, EndColumn = 1, Custom = "mmm-yy", FontBold = true, ValueFormula = "=DATE(YEAR(TODAY()),MONTH(TODAY()),1)", HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 1, StartColumn = 6, EndColumn = 17, Value = "FY 2011 - 2012", FontBold = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 1, StartColumn = 18, EndColumn = 29, Value = "FY 2012 - 2013", FontBold = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 1, StartColumn = 30, EndColumn = 41, Value = "FY 2013 - 2014", FontBold = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 1, StartColumn = 42, EndColumn = 53, Value = "FY 2014 - 2015", FontBold = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 1, StartColumn = 42, EndColumn = 53, Value = "FY 2015 - 2016", FontBold = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 1, StartColumn = 54, EndColumn = 65, Value = "FY 2016 - 2017", FontBold = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 1, StartColumn = 66, EndColumn = 77, Value = "FY 2017 - 2018", FontBold = true, HorizontalAlign = TextAlignmentTypeValues.Center)]
    [WorksheetRow(Row = 2, StartColumn = 0, EndColumn = 0, ValueExternal = InjectorNames.CurrentMonthYear)]
    [WorksheetRow(StartColumn = 0, EndColumn = 0, Value = "Extra")]
    public class ProjectOnOffChargeDefinition
    {
        [WorksheetColumn(Ordinal = 1, Name = "Project Manager", Width = 23.29D)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = ColorValues.Magenta,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public string ProjectManager { get; set; }

        [WorksheetColumn(Ordinal = 2, Name = "Portfolio", Width = 15.57D)]
        [WorksheetColumnStyle(HorizontalAlign = TextAlignmentTypeValues.Center)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.TangerineYellow,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public string Portfolio { get; set; }

        [WorksheetColumn(Ordinal = 3, Name = "Project Ref", Width = 18.14D)]
        [WorksheetColumnStyle(HorizontalAlign = TextAlignmentTypeValues.Center)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.TangerineYellow,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin,
            BottomBorderColor = ColorValues.Black, BottomBorderLineStyle = CellBorderTypeValues.Thin)]
        public string ProjectRef { get; set; }

        [WorksheetColumn(Ordinal = 4, Name = "ICD/SIC/CR Ref", Width = 18.43D)]
        [WorksheetColumnStyle(HorizontalAlign = TextAlignmentTypeValues.Center)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.TangerineYellow,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public string ChangeRef { get; set; }

        [WorksheetColumn(Ordinal = 5, Name = "Off or On Charge", Width = 23.43D)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.TangerineYellow,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public string ChargeType { get; set; }

        [WorksheetColumn(Ordinal = 6, Name = "Assets", Width = 13.57D)]
        [WorksheetColumnStyle(HorizontalAlign = TextAlignmentTypeValues.Center)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.TangerineYellow,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public int Assets { get; set; }

        [WorksheetColumn(Ordinal = 100, Name = "Apr-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Apr12 { get; set; }

        [WorksheetColumn(Ordinal = 101, Name = "May-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? May12 { get; set; }

        [WorksheetColumn(Ordinal = 102, Name = "Jun-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jun12 { get; set; }

        [WorksheetColumn(Ordinal = 103, Name = "Jul-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jul12 { get; set; }

        [WorksheetColumn(Ordinal = 104, Name = "Aug-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Aug12 { get; set; }

        [WorksheetColumn(Ordinal = 105, Name = "Sep-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Sep12 { get; set; }

        [WorksheetColumn(Ordinal = 106, Name = "Oct-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Oct12 { get; set; }

        [WorksheetColumn(Ordinal = 107, Name = "Nov-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Nov12 { get; set; }

        [WorksheetColumn(Ordinal = 108, Name = "Dec-12", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Dec12 { get; set; }

        [WorksheetColumn(Ordinal = 109, Name = "Jan-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jan13 { get; set; }

        [WorksheetColumn(Ordinal = 110, Name = "Feb-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Feb13 { get; set; }

        [WorksheetColumn(Ordinal = 111, Name = "Mar-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Mar13 { get; set; }

        [WorksheetColumn(Ordinal = 112, Name = "Apr-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Apr13 { get; set; }

        [WorksheetColumn(Ordinal = 113, Name = "May-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? May13 { get; set; }

        [WorksheetColumn(Ordinal = 114, Name = "Jun-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jun13 { get; set; }

        [WorksheetColumn(Ordinal = 115, Name = "Jul-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jul13 { get; set; }

        [WorksheetColumn(Ordinal = 116, Name = "Aug-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Aug13 { get; set; }

        [WorksheetColumn(Ordinal = 117, Name = "Sep-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Sep13 { get; set; }

        [WorksheetColumn(Ordinal = 118, Name = "Oct-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Oct13 { get; set; }

        [WorksheetColumn(Ordinal = 119, Name = "Nov-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Nov13 { get; set; }

        [WorksheetColumn(Ordinal = 120, Name = "Dec-13", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Dec13 { get; set; }

        [WorksheetColumn(Ordinal = 121, Name = "Jan-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jan14 { get; set; }

        [WorksheetColumn(Ordinal = 122, Name = "Feb-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Feb14 { get; set; }

        [WorksheetColumn(Ordinal = 123, Name = "Mar-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Mar14 { get; set; }

        [WorksheetColumn(Ordinal = 124, Name = "Apr-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Apr14 { get; set; }

        [WorksheetColumn(Ordinal = 125, Name = "May-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? May14 { get; set; }

        [WorksheetColumn(Ordinal = 126, Name = "Jun-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jun14 { get; set; }

        [WorksheetColumn(Ordinal = 127, Name = "Jul-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jul14 { get; set; }

        [WorksheetColumn(Ordinal = 128, Name = "Aug-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Aug14 { get; set; }

        [WorksheetColumn(Ordinal = 129, Name = "Sep-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Sep14 { get; set; }

        [WorksheetColumn(Ordinal = 130, Name = "Oct-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Oct14 { get; set; }

        [WorksheetColumn(Ordinal = 131, Name = "Nov-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Nov14 { get; set; }

        [WorksheetColumn(Ordinal = 132, Name = "Dec-14", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Dec14 { get; set; }

        [WorksheetColumn(Ordinal = 133, Name = "Jan-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jan15 { get; set; }

        [WorksheetColumn(Ordinal = 134, Name = "Feb-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Feb15 { get; set; }

        [WorksheetColumn(Ordinal = 135, Name = "Mar-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Mar15 { get; set; }

        [WorksheetColumn(Ordinal = 136, Name = "Apr-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Apr15 { get; set; }

        [WorksheetColumn(Ordinal = 137, Name = "May-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? May15 { get; set; }

        [WorksheetColumn(Ordinal = 138, Name = "Jun-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jun15 { get; set; }

        [WorksheetColumn(Ordinal = 139, Name = "Jul-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jul15 { get; set; }

        [WorksheetColumn(Ordinal = 140, Name = "Aug-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Aug15 { get; set; }

        [WorksheetColumn(Ordinal = 141, Name = "Sep-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Sep15 { get; set; }

        [WorksheetColumn(Ordinal = 142, Name = "Oct-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Oct15 { get; set; }

        [WorksheetColumn(Ordinal = 143, Name = "Nov-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Nov15 { get; set; }

        [WorksheetColumn(Ordinal = 144, Name = "Dec-15", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Dec15 { get; set; }

        [WorksheetColumn(Ordinal = 145, Name = "Jan-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jan16 { get; set; }

        [WorksheetColumn(Ordinal = 146, Name = "Feb-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Feb16 { get; set; }

        [WorksheetColumn(Ordinal = 147, Name = "Mar-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Mar16 { get; set; }

        [WorksheetColumn(Ordinal = 148, Name = "Apr-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Apr16 { get; set; }

        [WorksheetColumn(Ordinal = 149, Name = "May-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? May16 { get; set; }

        [WorksheetColumn(Ordinal = 150, Name = "Jun-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jun16 { get; set; }

        [WorksheetColumn(Ordinal = 151, Name = "Jul-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jul16 { get; set; }

        [WorksheetColumn(Ordinal = 152, Name = "Aug-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Aug16 { get; set; }

        [WorksheetColumn(Ordinal = 153, Name = "Sep-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Sep16 { get; set; }

        [WorksheetColumn(Ordinal = 154, Name = "Oct-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Oct16 { get; set; }

        [WorksheetColumn(Ordinal = 155, Name = "Nov-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Nov16 { get; set; }

        [WorksheetColumn(Ordinal = 156, Name = "Dec-16", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Dec16 { get; set; }

        [WorksheetColumn(Ordinal = 157, Name = "Jan-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jan17 { get; set; }

        [WorksheetColumn(Ordinal = 158, Name = "Feb-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Feb17 { get; set; }

        [WorksheetColumn(Ordinal = 159, Name = "Mar-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Mar17 { get; set; }

        [WorksheetColumn(Ordinal = 160, Name = "Apr-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Apr17 { get; set; }

        [WorksheetColumn(Ordinal = 161, Name = "May-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? May17 { get; set; }

        [WorksheetColumn(Ordinal = 162, Name = "Jun-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jun17 { get; set; }

        [WorksheetColumn(Ordinal = 163, Name = "Jul-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jul17 { get; set; }

        [WorksheetColumn(Ordinal = 164, Name = "Aug-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Aug17 { get; set; }

        [WorksheetColumn(Ordinal = 165, Name = "Sep-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Sep17 { get; set; }

        [WorksheetColumn(Ordinal = 166, Name = "Oct-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Oct17 { get; set; }

        [WorksheetColumn(Ordinal = 167, Name = "Nov-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Nov17 { get; set; }

        [WorksheetColumn(Ordinal = 168, Name = "Dec-17", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Dec17 { get; set; }

        [WorksheetColumn(Ordinal = 169, Name = "Jan-18", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Jan18 { get; set; }

        [WorksheetColumn(Ordinal = 170, Name = "Feb-18", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Feb18 { get; set; }

        [WorksheetColumn(Ordinal = 171, Name = "Mar-18", Width = 10D)]
        [WorksheetColumnStyle(Custom = ReportFormatValues.CurrencyFormat)]
        [WorksheetColumnHeaderStyle(FontBold = true, Pattern = BackgroundTypeValues.Solid, ForegroundColor = HexColorNames.VividBlue, FontColor = ColorValues.White,
            TopBorderColor = ColorValues.Black, TopBorderLineStyle = CellBorderTypeValues.Thin,
            LeftBorderColor = ColorValues.Black, LeftBorderLineStyle = CellBorderTypeValues.Thin,
            RightBorderColor = ColorValues.Black, RightBorderLineStyle = CellBorderTypeValues.Thin)]
        public decimal? Mar18 { get; set; }
    }
}