using System.Collections.Generic;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    public class AddResolverLevelZeroViewModel : LevelViewModel
    {
        public AddResolverLevelZeroViewModel()
        {
            ServiceDesks = new List<SelectListItem>();
            ServiceDeliveryOrganisationTypes = new List<SelectListItem>();
            ServiceDeliveryUnitTypes = new List<SelectListItem>();
            ResolverGroupTypes = new List<SelectListItem>();
        }

        public int Id { get; set; }
        public bool IsSingleDeskContext { get; set; }
        public int SingleDeskId { get; set; }
        public List<SelectListItem> ServiceDesks { get; private set; }
        public List<SelectListItem> ServiceDeliveryOrganisationTypes { get; private set; }
        public List<SelectListItem> ServiceDeliveryUnitTypes { get; private set; }
        public List<SelectListItem> ResolverGroupTypes { get; private set; }
    }
}