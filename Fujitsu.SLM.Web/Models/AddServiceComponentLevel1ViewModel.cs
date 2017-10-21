using System.Collections.Generic;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    public class AddServiceComponentLevel1ViewModel : LevelViewModel
    {
        public AddServiceComponentLevel1ViewModel()
        {
            ServiceFunctions = new List<SelectListItem>();
        }

        public int? ServiceFunctionId { get; set; }
        public string ServiceFunctionName { get; set; }
        public bool HasServiceFunctionContext { get; set; }

        public List<SelectListItem> ServiceFunctions { get; private set; }
    }
}