using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets
{
    public class WorksheetRowItemAttribute : Attribute
    {
        public Int32 StartRow { get; private set; }

        public Int32 TitleRow { get; private set; }

        public Int32 AlternativeTitleRow { get; private set; }

        public Int32 EndRow { get; set; }

        public bool StopAtFirstEmptyRow { get; set; }

        public bool DuplicateFirstRowOnGenerate { get; set; }

        public WorksheetRowItemAttribute(int startRow, int titleRow)
        {
            StartRow = startRow;
            TitleRow = titleRow;
            AlternativeTitleRow = titleRow;
        }

        public WorksheetRowItemAttribute(int startRow, int titleRow, int alternativeTitleRow)
        {
            StartRow = startRow;
            TitleRow = titleRow;
            AlternativeTitleRow = alternativeTitleRow;
        }
    }
}
