using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.Aspose.Spreadsheets.Tests.TestClasses
{
    [WorksheetItem]
    public class Order
    {
        [WorksheetItemValue("C3")]
        [Required]
        public int OrderId { get; set; }

        [WorksheetItemValue("C4")]
        [Required]
        public String OrderRef { get; set; }

        [WorksheetItemValue("C5")]
        [Required]
        public DateTime OrderDate { get; set; }

        [WorksheetItemValue("C6")]
        [Range(10, 5000)]
        public Decimal OrderTotal { get; set; }

        public List<OrderItem> OrderItems { get; set; }

    }
}
