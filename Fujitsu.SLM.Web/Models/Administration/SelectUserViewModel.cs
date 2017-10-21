using System.ComponentModel;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(SelectUserViewModelValidator))]
    public class SelectUserViewModel
    {
        [DisplayName("User")]
        public string SelectedEmail { get; set; }

        public string RoleName { get; set; }
    }
}