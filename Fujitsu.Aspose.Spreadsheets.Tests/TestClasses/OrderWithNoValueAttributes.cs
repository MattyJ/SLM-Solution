using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetItem]
    public class OrderWithNoValueAttributes
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public String OrderRef { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Range(10, 5000)]
        public Decimal OrderTotal { get; set; }

    }
}
