using System;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    public class InvalidUser
    {
        [WorksheetRowItemValue("Title")]
        public String Title { get; set; }

        [WorksheetRowItemValue("GivenName")]
        public String FirstName { get; set; }

        [WorksheetRowItemValue("LastName")]
        public String Surname { get; set; }
    }
}
