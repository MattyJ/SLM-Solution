using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(MoveResolverLevelZeroViewModelValidator))]
    public class MoveResolverLevelZeroViewModel
    {
        public MoveResolverLevelZeroViewModel()
        {
            Desks = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Display(Name = "Service Desk")]
        public int DestinationDeskId { get; set; }

        public List<SelectListItem> Desks { get; set; }
    }
}
