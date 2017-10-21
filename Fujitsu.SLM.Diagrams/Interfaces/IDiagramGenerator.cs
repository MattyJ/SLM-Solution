using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Diagrams.Entities;

namespace Fujitsu.SLM.Diagrams.Interfaces
{
    public interface IDiagramGenerator
    {
        List<ChartDataListItem> Generate(int serviceDeskId,
            bool svcDomains = false,
            bool svcFunctions = false,
            bool svcComponents = false,
            bool resolvers = false,
            bool svcActivities = false,
            bool opProcs = false,
            string[] domainsSelected = null);
    }
}
