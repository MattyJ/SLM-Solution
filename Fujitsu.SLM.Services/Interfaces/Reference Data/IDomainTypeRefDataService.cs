using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IDomainTypeRefDataService : IService<DomainTypeRefData>
    {
        IEnumerable<DomainTypeRefData> GetAllAndNotVisibleForCustomer(int customerId);
        int GetNumberOfDomainTypeReferences(int id);
        IEnumerable<DomainTypeRefDataListItem> GetDomainTypeRefData(bool isAdmin, string emailAddress);
        DomainTypeRefData InsertorUpdate(string typeName);
    }
}