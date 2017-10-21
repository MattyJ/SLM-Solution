using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(BaseServiceDeliveryOrganisationTypeViewModelValidator))]
    public abstract class BaseServiceDeliveryOrganisationTypeViewModel : InsertedUpdatedViewModel
    {
        [Display(Name = "Service Delivery Organisation")]
        public int? ServiceDeliveryOrganisationTypeId { get; set; }

        [Display(Name = "Service Delivery Organisation Name")]
        public string ServiceDeliveryOrganisationName { get; set; }

        [Display(Name = "Notes")]
        public string ServiceDeliveryOrganisationNotes { get; set; }
    }
}