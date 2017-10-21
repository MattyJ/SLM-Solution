using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Web.Models.Menu
{
    public class LevelOneMenuModel
    {
        public bool ServiceDecompositionsSelected { get; set; }

        public bool AddServiceDeskSelected { get; set; }
        public bool ServiceDeskInputsSelected { get; set; }
        public bool ServiceDomainsSelected { get; set; }
        public bool ServiceFunctionsSelected { get; set; }
        public bool ServiceComponentsSelected { get; set; }
        public bool ResoversSelected { get; set; }
        public bool ProcessDotMatrixSelected { get; set; }
        public bool DiagramsSelected { get; set; }
    }
}
