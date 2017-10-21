using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.UI;

namespace Fujitsu.SLM.Web.Models
{
    public class ChartViewModel
    {
        public string Title { get; set; }
        public string Subject { get; set; }
        public string Filename { get; set; }
        public string Creator { get; set; }
        public string Author { get; set; }
        public int Level { get; set; }
        public int Id { get; set; }
        public bool ServiceDomains { get; set; }
        public bool ServiceFunctions { get; set; }
        public bool ServiceComponents { get; set; }
        public bool Resolvers { get; set; }
        public bool ServiceActivities { get; set; }
        public bool OperationalProcesses { get; set; }
        public string[] DomainsSelected { get; set; }
        public IEnumerable<TreeViewItemModel> InlineDomainData { get; set; }


    }
}