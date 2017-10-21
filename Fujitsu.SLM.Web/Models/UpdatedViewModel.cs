using System;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public abstract class UpdatedViewModel : BaseViewModel
    {
        [Display(Name = "Updated By")]
        public string UpdatedBy { get; set; }

        [Display(Name = "Last Updated")]
        public DateTime UpdatedDate { get; set; }

    }
}