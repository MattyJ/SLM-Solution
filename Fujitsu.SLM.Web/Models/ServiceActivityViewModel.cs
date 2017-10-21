using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ServiceActivityViewModelValidator))]
    public class ServiceActivityViewModel
    {
        [Display(Name = "Service Activities")]
        public string ServiceActivities { get; set; }
    }
}