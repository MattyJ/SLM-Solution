using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(MoveServiceComponentLevel1ViewModelValidator))]
    public class MoveServiceComponentLevel1ViewModel
    {
        public MoveServiceComponentLevel1ViewModel()
        {
            ServiceFunctions = new List<SelectListItem>();
        }

        public int ServiceComponentId { get; set; }

        [Display(Name = "Service Function")]
        public int DestinationServiceFunctionId { get; set; }

        public List<SelectListItem> ServiceFunctions { get; set; }
    }
}