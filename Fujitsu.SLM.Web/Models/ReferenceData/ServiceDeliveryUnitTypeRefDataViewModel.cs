using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ServiceDeliveryUnitTypeRefDataViewModelValidator))]
    public class ServiceDeliveryUnitTypeRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Delivery Unit Name")]
        public string ServiceDeliveryUnitTypeName { get; set; }

        [Display(Name = "Visible In Lookups?")]
        public bool Visible { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }
    }
}