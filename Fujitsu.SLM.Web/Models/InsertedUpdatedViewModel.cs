using System;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public abstract class InsertedUpdatedViewModel : UpdatedViewModel
    {
        [Display(Name = "Created By")]
        public string InsertedBy { get; set; }

        [Display(Name = "Created")]
        public DateTime InsertedDate { get; set; }
    }

}