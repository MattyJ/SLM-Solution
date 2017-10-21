using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

// ReSharper disable once CheckNamespace
namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ParameterViewModelValidator))]
    public class ParameterViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Parameter Name")]
        public string ParameterName { get; set; }

        [Display(Name = "Parameter Type")]
        public string Type { get; set; }

        [Display(Name = "Parameter Value")]
        public string ParameterValue { get; set; }
    }
}