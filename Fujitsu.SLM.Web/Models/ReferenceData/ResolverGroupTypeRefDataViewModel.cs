using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ResolverGroupTypeRefDataViewModelValidator))]
    public class ResolverGroupTypeRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Resolver Group Name")]
        public string ResolverGroupTypeName { get; set; }

        [Display(Name = "Visible In Lookups?")]
        public bool Visible { get; set; }

        [Display(Name = "Sort Order")]
        public int Order { get; set; }  // Two identical Kendo Windows are used and JavaScript is only able to access the first rendered element with a given ID

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }
    }
}
