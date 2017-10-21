using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Web.Models
{
    public class ModalWaitViewModel
    {
        public ModalWaitViewModel()
        {
            ButtonClicks = new string[] { };
        }
        public string[] ButtonClicks { get; set; }
    }
}
