using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(MoveServiceComponentLevel2ViewModelValidator))]
    public class MoveServiceComponentLevel2ViewModel
    {
        public MoveServiceComponentLevel2ViewModel()
        {
            ServiceComponents = new List<SelectListItem>();
        }

        public int ServiceComponentId { get; set; }

        [Display(Name = "Service Component")]
        public int DestinationServiceComponentId { get; set; }

        public List<SelectListItem> ServiceComponents { get; set; }
    }
}