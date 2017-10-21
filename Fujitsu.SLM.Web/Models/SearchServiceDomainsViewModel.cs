using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fujitsu.SLM.Web.Models
{
    public class SearchServiceDomainsViewModel
    {
        public SearchServiceDomainViewModel ServiceDomain { get; set; }

        public IList<SearchServiceDomainViewModel> SearchResults { get; set; }

        public SearchServiceDomainsViewModel()
        {
            ServiceDomain = new SearchServiceDomainViewModel();
            SearchResults = new List<SearchServiceDomainViewModel>();
        }
    }
}
