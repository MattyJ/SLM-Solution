using System.Collections.Generic;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    public class AddServiceComponentLevel2ViewModel : LevelViewModel
    {
        public AddServiceComponentLevel2ViewModel()
        {
            ServiceComponents = new List<SelectListItem>();
        }

        public int ServiceComponentId { get; set; }
        public string ServiceComponentName { get; set; }

        public int? ServiceFunctionId { get; set; }
        public bool HasServiceFunctionContext { get; set; }

        public List<SelectListItem> ServiceComponents { get; private set; }
    }
}