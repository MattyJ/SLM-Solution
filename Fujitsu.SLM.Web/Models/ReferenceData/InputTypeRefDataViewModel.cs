using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(InputTypeRefDataViewModelValidator))]
    public class InputTypeRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Input Type Number")]
        public int InputTypeNumber { get; set; }

        [Display(Name = "Input Type Name")]
        public string InputTypeName { get; set; }

        [Display(Name = "Sort Order")]
        public int SortOrder { get; set; }

        [Display(Name = "Default")]
        public bool Default { get; set; }

        [Display(Name = "Usage Count")]
        public int UsageCount { get; set; }
    }
}
