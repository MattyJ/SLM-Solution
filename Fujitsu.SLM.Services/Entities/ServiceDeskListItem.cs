using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Services.Entities
{
    public class ServiceDeskListItem
    {
        public int Id { get; set; }

        public string DeskName { get; set; }

        public string UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int ServiceDecompositionId { get; set; }

        public string ServiceDecompositionName { get; set; }

        public IEnumerable<InputTypeRefData> DeskInputTypes { get; set; }
    }
}
