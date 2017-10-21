using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(BaseServiceDeliveryOrganisationViewModelValidator))]
    public abstract class BaseServiceDeliveryOrganisationViewModel : InsertedUpdatedViewModel
    {
        [Display(Name = "SDO Type")]
        public int? ServiceDeliveryOrganisationTypeId { get; set; }

        [Display(Name = "Notes")]
        public string ServiceDeliveryOrganisationNotes { get; set; }

    }
}