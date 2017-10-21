using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(AuditViewModelValidator))]
    public class AuditViewModel : InsertedUpdatedViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Version Number")]
        public double Version { get; set; }
        [Display(Name = "Version Number")]
        public string VersionNumber => $"v{Version:0.0}";
        [Display(Name = "Reason For Issue")]
        public string ReasonForIssue { get; set; }
        public string Notes { get; set; }
        public int CustomerId { get; set; }
    }
}