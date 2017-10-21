using System;
using Fujitsu.Aspose.Spreadsheets.Attributes;
using Fujitsu.Aspose.Spreadsheets.Constants;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses.ReportDefinitions
{
    [Worksheet(Name = "Asset FRD", TabColor = ColorValues.Yellow, StandardWidth = 25)]
    public class AssetFrdDefinition : MonthCurrencySummaryDefinition
    {
        [WorksheetColumn(Ordinal = 1, Name = "Change Ref")]
        public string ChangeRef { get; set; }

        [WorksheetColumn(Ordinal = 2, Name = "Asset Tag")]
        public string AssetTag { get; set; }

        [WorksheetColumn(Ordinal = 3, Name = "Server Name")]
        public string ServerName { get; set; }

        [WorksheetColumn(Ordinal = 4, Name = "On Charge Date")]
        [WorksheetColumnStyle(Custom = "dd/MM/yyyy")]
        public DateTime? OnChargeDate { get; set; }

        [WorksheetColumn(Ordinal = 5, Name = "Off Charge Date")]
        [WorksheetColumnStyle(Custom = "dd/MM/yyyy")]
        public DateTime? OffChargeDate { get; set; }

        [WorksheetColumn(Ordinal = 200, Name = "Project Manager")]
        public string ProjectManager { get; set; }

        [WorksheetColumn(Ordinal = 201, Name = "Portfolio")]
        public string Portfolio { get; set; }

        [WorksheetColumn(Ordinal = 202, Name = "Project Ref")]
        public string ProjectRef { get; set; }

        [WorksheetColumn(Ordinal = 203, Name = "Project Status")]
        public string ProjectStatus { get; set; }
    }
}