using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IResolverGroupTypeRefDataService : IService<ResolverGroupTypeRefData>
    {
        IEnumerable<ResolverGroupTypeRefData> GetAllAndNotVisibleForCustomer(int customerId);
        int GetNumberOfResolverGroupTypeReferences(int id);
        IEnumerable<ResolverGroupTypeRefDataListItem> GetResolverGroupTypeRefDataWithUsageStats();
        ResolverGroupTypeRefData InsertorUpdate(string typeName);
    }
}