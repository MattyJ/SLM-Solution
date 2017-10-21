using System;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class ContributorViewModel : UpdatedViewModel
    {
        public int Id { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }

        [Display(Name = "User")]
        public string EmailAddress { get; set; }

        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
    }
}