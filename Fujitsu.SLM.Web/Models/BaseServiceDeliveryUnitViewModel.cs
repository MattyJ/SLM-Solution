using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(BaseServiceDeliveryUnitViewModelValidator))]
    public abstract class BaseServiceDeliveryUnitViewModel : InsertedUpdatedViewModel
    {
        [Display(Name = "SDU Type")]
        public int? ServiceDeliveryUnitTypeId { get; set; }

        [Display(Name = "Notes")]
        public string ServiceDeliveryUnitNotes { get; set; }
    }
}