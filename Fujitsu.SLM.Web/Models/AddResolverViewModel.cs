using System.Collections.Generic;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    public class AddResolverViewModel : LevelViewModel
    {
        public AddResolverViewModel()
        {
            ServiceComponents = new List<SelectListItem>();
            ServiceDeliveryOrganisationTypes = new List<SelectListItem>();
            ServiceDeliveryUnitTypes = new List<SelectListItem>();
            ResolverGroupTypes = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public int? ServiceComponentId { get; set; }
        public string ServiceComponentName { get; set; }
        public bool HasServiceComponentContext { get; set; }
        public List<SelectListItem> ServiceComponents { get; private set; }
        public List<SelectListItem> ServiceDeliveryOrganisationTypes { get; private set; }
        public List<SelectListItem> ServiceDeliveryUnitTypes { get; private set; }
        public List<SelectListItem> ResolverGroupTypes { get; private set; }
    }
}