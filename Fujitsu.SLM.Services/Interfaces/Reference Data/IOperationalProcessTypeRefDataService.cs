using Fujitsu.SLM.Model;
using System.Collections.Generic;
using Fujitsu.SLM.Services.Entities;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IOperationalProcessTypeRefDataService : IService<OperationalProcessTypeRefData>
    {
        int GetNumberOfOperationalProcessTypeReferences(int id);
        IEnumerable<OperationalProcessTypeRefData> GetAllAndNotVisibleForCustomer(int customerId);
        void PurgeOrphans();
        IEnumerable<OperationalProcessTypeRefDataListItem> GetOperationalProcessTypeRefDataWithUsageStats();
    }
}