using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;


namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(UploadCustomerFileViewModelValidator))]
    public class UploadCustomerFileViewModel : LevelViewModel
    {
        [DisplayName("Customer File")]
        public HttpPostedFileBase CustomerFile { get; set; }

        public bool DisplayLevel { get; set; }

        public List<SelectListItem> Levels { get; set; }

        [DisplayName("Notes")]
        public string Notes { get; set; }
    }
}