using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ServiceDeliveryOrganisationTypeRefDataViewModelValidator))]
    public class ServiceDeliveryOrganisationTypeRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Service Delivery Organisation Name")]
        public string ServiceDeliveryOrganisationTypeName { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }
    }
}
