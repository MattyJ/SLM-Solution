using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(DiagramViewModelValidator))]
    public class DiagramViewModel : InsertedUpdatedViewModel
    {
        public int Id { get; set; }

        public int Level { get; set; }

        public string Filename { get; set; }

        [DisplayName("Notes")]
        public string DiagramNotes { get; set; }
    }
}