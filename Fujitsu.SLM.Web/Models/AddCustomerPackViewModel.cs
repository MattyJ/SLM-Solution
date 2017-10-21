using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(AddCustomerPackViewModelValidator))]
    public class AddCustomerPackViewModel : LevelViewModel
    {
        public string Filename { get; set; }

        [DisplayName("Pack Notes")]
        public string PackNotes { get; set; }

        public bool DisplayLevel { get; set; }

        public List<SelectListItem> Levels { get; set; }
    }
}
