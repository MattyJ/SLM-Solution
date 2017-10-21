using System;
using System.Collections.Generic;
using System.Drawing;
using Aspose.Cells;

namespace Fujitsu.Aspose.Spreadsheets
{
    public static class WorksheetExtensions
    {
        public static Cell FindCellWithValue(this Worksheet worksheet, string valueToFind)
        {
            var opts = new FindOptions {LookInType = LookInType.Values, LookAtType = LookAtType.EntireContent};
            return worksheet.Cells.Find(valueToFind, null, opts);
        }

       

    }
}