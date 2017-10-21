using System.Collections.Generic;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    public class AddServiceDomainViewModel : LevelViewModel
    {
        public AddServiceDomainViewModel()
        {
            ServiceDesks = new List<SelectListItem>();
            DomainTypes = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public int ServiceDeskId { get; set; }
        public string ServiceDeskName { get; set; }
        public bool HasServiceDeskContext { get; set; }
        public List<SelectListItem> ServiceDesks { get; private set; }
        public List<SelectListItem> DomainTypes { get; private set; }
    }
}