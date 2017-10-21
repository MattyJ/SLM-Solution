using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(CustomerViewModelValidator))]
    public class CustomerViewModel : UpdatedViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Notes")]
        [DataType(DataType.MultilineText)]
        [UIHint("WindowMultiLine")]
        public string CustomerNotes { get; set; }

        [Display(Name = "Active")]
        public bool Active { get; set; }

        [Display(Name = "Owner")]
        public string AssignedArchitect { get; set; }

        public bool Owner { get; set; }

        [Display(Name = "Stage")]
        public bool Baseline { get; set; }

        public string ReturnUrl { get; set; }
    }
}
