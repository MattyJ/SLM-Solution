using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(DomainTypeRefDataViewModelValidator))]
    public class DomainTypeRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Domain Name")]
        public string DomainName { get; set; }

        [Display(Name = "Visible In Lookups?")]
        public bool Visible { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }
    }
}