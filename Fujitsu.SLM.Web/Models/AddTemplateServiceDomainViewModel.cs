using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(AddTemplateServiceDomainViewModelValidator))]
    public class AddTemplateServiceDomainViewModel : LevelViewModel
    {
        public AddTemplateServiceDomainViewModel()
        {
            ServiceDesks = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Display(Name = "Service Desk")]
        public int ServiceDeskId { get; set; }
        public string TemplateType { get; set; }
        public string ServiceDeskName { get; set; }
        public bool HasServiceDeskContext { get; set; }
        public List<SelectListItem> ServiceDesks { get; private set; }
    }
}