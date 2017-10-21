using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ServiceDomainViewModelValidator))]
    public class ServiceDomainViewModel : BaseServiceDomainViewModel
    {
        [Display(Name = "Service Desk")]
        public int ServiceDeskId { get; set; }
    }
}