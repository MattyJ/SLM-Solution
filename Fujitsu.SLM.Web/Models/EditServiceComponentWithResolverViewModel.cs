using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public abstract class EditServiceComponentWithResolverViewModel : EditServiceComponentViewModel
    {

        [UIHint("ServiceDeliveryOrganisationViewModel")]
        public EditResolverServiceDeliveryOrganisationViewModel ResolverServiceDeliveryOrganisation { get; set; }

        [UIHint("ServiceDeliveryUnitViewModel")]
        public EditResolverServiceDeliveryUnitViewModel ResolverServiceDeliveryUnit { get; set; }

        [UIHint("ResolverGroupViewModel")]
        public EditResolverResolverGroupViewModel ResolverGroup { get; set; }
    }
}