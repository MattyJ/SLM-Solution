using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(RegionTypeRefDataViewModelValidator))]
    public class RegionTypeRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Region Name")]
        public string RegionTypeName { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }
    }
}