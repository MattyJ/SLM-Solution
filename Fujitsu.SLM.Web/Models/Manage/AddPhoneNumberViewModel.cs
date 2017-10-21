﻿using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }
}