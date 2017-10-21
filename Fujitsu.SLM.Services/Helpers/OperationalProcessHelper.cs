using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services.Helpers
{
    public static class OperationalProcessHelper
    {
        public static List<OperationalProcessTypeRefData> GetStandardAndSelectedForCustomer(IRepository<OperationalProcessTypeRefData> operationalProcessRefDataRepository,
            IRepository<OperationalProcessType> operationalProcessTypeRepository,
            int customerId)
        {
            var result = operationalProcessRefDataRepository.Query(x => x.Standard).OrderBy(x => x.SortOrder).ThenBy(x => x.OperationalProcessTypeName).ToList();

            result.AddRange(operationalProcessTypeRepository
                .Query(x => x.Resolver.ServiceDesk.CustomerId == customerId && !x.OperationalProcessTypeRefData.Standard)
                    .Select(x => x.OperationalProcessTypeRefData).Distinct()
                    .OrderBy(x => x.SortOrder).ThenBy(x => x.OperationalProcessTypeName)
                    .ToList());

            return result;
        }

        public static List<OperationalProcessTypeRefData> GetAllAndNotVisibleForCustomer(IRepository<OperationalProcessTypeRefData> operationalProcessRefDataRepository,
            IRepository<OperationalProcessType> operationalProcessTypeRepository,
            int customerId)
        {
            var result = operationalProcessRefDataRepository.Query(x => x.Visible).OrderBy(x => x.Standard).ThenBy(x => x.SortOrder).ThenBy(x => x.OperationalProcessTypeName).ToList();

            result.AddRange(operationalProcessTypeRepository
                .Query(x => x.Resolver.ServiceDesk.CustomerId == customerId && !x.OperationalProcessTypeRefData.Visible)
                    .Select(x => x.OperationalProcessTypeRefData).Distinct()
                    .OrderBy(x => x.SortOrder).ThenBy(x => x.OperationalProcessTypeName)
                    .ToList());

            return result;
        }
    }
}
