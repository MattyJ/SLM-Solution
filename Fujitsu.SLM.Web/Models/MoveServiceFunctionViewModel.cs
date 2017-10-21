using System.Collections.Generic;
using System.ComponentModel;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(MoveServiceFunctionViewModelValidator))]
    public class MoveServiceFunctionViewModel
    {
        public MoveServiceFunctionViewModel()
        {
            ServiceDomains = new List<KeyValuePair<int, string>>();
        }

        public int ServiceFunctionId { get; set; }

        [DisplayName("Service Domain")]
        public int ServiceDomainId { get; set; }

        [DisplayName("Service Domain")]
        public List<KeyValuePair<int, string>> ServiceDomains { get; private set; }
    }
}