using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using FluentValidation.Attributes;
using Fujitsu.SLM.Web.Validators;

// ReSharper disable once CheckNamespace
namespace Fujitsu.SLM.Web.Models
{
    [Validator(typeof(RoleViewModelValidator))]
    public class RoleViewModel
    {
        public string RoleName { get; set; }

        [DisplayName("User")]
        public string UserId { get; set; }
    }
}