using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Web.Models.Session
{
    public class AppContext
    {
        public CurrentCustomerViewModel CurrentCustomer { get; set; }
        public AppContext()
        {
            CurrentCustomer = new CurrentCustomerViewModel();
        }
    }
}
