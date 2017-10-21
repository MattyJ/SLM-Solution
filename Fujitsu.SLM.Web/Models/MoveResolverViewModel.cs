using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(MoveResolverViewModelValidator))]
    public class MoveResolverViewModel
    {
        public MoveResolverViewModel()
        {
            ServiceComponents = new List<SelectListItem>();
        }

        public int ServiceComponentId { get; set; }

        [Display(Name = "Service Component")]
        public int DestinationServiceComponentId { get; set; }

        public List<SelectListItem> ServiceComponents { get; set; }
    }
}