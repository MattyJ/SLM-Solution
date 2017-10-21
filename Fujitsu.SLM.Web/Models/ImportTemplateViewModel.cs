using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ImportTemplateViewModelValidator))]
    public class ImportTemplateViewModel
    {
        [Display(Name = "Template Type")]
        public string TemplateType { get; set; }

        [Display(Name = "Template File")]
        public HttpPostedFileBase SpreadsheetFile { get; set; }
    }
}