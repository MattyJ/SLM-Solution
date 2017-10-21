using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(OperationalProcessTypeRefDataViewModelValidator))]
    public class OperationalProcessTypeRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Operational Process Name")]
        public string OperationalProcessTypeName { get; set; }

        [Display(Name = "Visible In Lookups?")]
        public bool Visible { get; set; }

        [Display(Name = "Standard")]
        public bool Standard { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }
    }
}