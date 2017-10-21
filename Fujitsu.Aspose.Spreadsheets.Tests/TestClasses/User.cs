using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetRowItem(1, 0, StopAtFirstEmptyRow = true)]
    public class User
    {
        [WorksheetRowItemValue("Title")]
        public String Title { get; set; }

        [WorksheetRowItemValue("GivenName")]
        public String FirstName { get; set; }

        [WorksheetRowItemValue(ColumnIndex = 1)]
        public String Surname { get; set; }

        [Range(22,80)]
        [WorksheetRowItemValue("Age")]
        public int Age { get; set; }

        [Required]
        [WorksheetRowItemValue("DateOfBirth")]
        public DateTime? DateOfBirth { get; set; }

        [WorksheetRowItemValue("Salary")]
        [Range(10.0, 45000.0)]
        public decimal Salary { get; set; }
    }
}
