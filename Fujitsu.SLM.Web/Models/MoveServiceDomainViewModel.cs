using System.Collections.Generic;
using System.ComponentModel;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(MoveServiceDomainViewModelValidator))]
    public class MoveServiceDomainViewModel
    {
        public MoveServiceDomainViewModel()
        {
            ServiceDesks = new List<KeyValuePair<int, string>>();
        }

        public int ServiceDomainId { get; set; }

        [DisplayName("Service Desk")]
        public int ServiceDeskId { get; set; }

        [DisplayName("Service Desk")]
        public List<KeyValuePair<int, string>> ServiceDesks { get; private set; }
    }
}