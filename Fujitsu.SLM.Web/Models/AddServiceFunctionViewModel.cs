using System.Collections.Generic;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    public class AddServiceFunctionViewModel : LevelViewModel
    {
        public AddServiceFunctionViewModel()
        {
            ServiceDomains = new List<SelectListItem>();
            FunctionTypes = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public int ServiceDomainId { get; set; }
        public string ServiceDomainName { get; set; }
        public bool HasServiceDomainContext { get; set; }
        public List<SelectListItem> ServiceDomains { get; private set; }
        public List<SelectListItem> FunctionTypes { get; private set; }
    }
}