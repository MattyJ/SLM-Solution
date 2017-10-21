using System;
using System.Diagnostics.CodeAnalysis;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [ExcludeFromCodeCoverage]
    [WorksheetRowItem(8, 7, DuplicateFirstRowOnGenerate = true)]
    public class SoftwareItemNoValueAttributes
    {
        public String Id { get; set; }

        public String SoftwareName { get; set; }

        public decimal Cost { get; set; }

        public int Quantity { get; set; }

        public decimal TotalCost { get { return Cost * Quantity; } }

    }
}