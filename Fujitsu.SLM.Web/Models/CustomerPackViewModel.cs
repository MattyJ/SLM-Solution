using System.ComponentModel;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(CustomerPackViewModelValidator))]
    public class CustomerPackViewModel : InsertedUpdatedViewModel
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public string Filename { get; set; }

        [DisplayName("Pack Notes")]
        public string PackNotes { get; set; }
    }
}