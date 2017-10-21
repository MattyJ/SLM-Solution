using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(ServiceDeskViewModelValidator))]
    public class ServiceDeskViewModel : UpdatedViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Desk Name")]
        public string DeskName { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Desk Input Types")]
        public List<InputTypeRefData> DeskInputTypes { get; set; }
    }
}