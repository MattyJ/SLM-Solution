using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fujitsu.SLM.Web.Models
{
    public class SearchServiceDomainViewModel : UpdatedViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name = "Decomposition Name")]
        public string DecompositionName { get; set; }

        [Display(Name = "Desk Name")]
        public string DeskName { get; set; }

        [Display(Name = "Domain Name")]
        public string DomainName { get; set; }

        public IList<ServiceFunction> ServiceFunctions { get; set; }
    }
}
