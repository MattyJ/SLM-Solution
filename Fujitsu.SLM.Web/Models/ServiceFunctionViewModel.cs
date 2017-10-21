using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ServiceFunctionViewModelValidator))]
    public class ServiceFunctionViewModel : BaseServiceFunctionViewModel
    {
        [Display(Name = "Service Domain")]
        public int ServiceDomainId { get; set; }
    }
}
