using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ContextHelpRefDataViewModelValidator))]
    public class ContextHelpRefDataViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Help Key")]
        public string Key { get; set; }

        [Display(Name = "Help Title")]
        public string Title { get; set; }

        [AllowHtml]
        [UIHint("HelpHtml")]
        [Display(Name = "Help Text")]
        public string HelpText { get; set; }

        [Display(Name = "Help Video File")]
        public HttpPostedFileBase HelpVideoFile { get; set; }
        public string HelpVideoTitle { get; set; }
        public string HelpVideoFileName { get; set; }
        public string HelpVideoFileExtension { get; set; }
        public bool HelpVideo => !string.IsNullOrEmpty(HelpVideoFileName);
    }
}
