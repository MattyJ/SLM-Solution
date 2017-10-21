using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IFunctionTypeRefDataService : IService<FunctionTypeRefData>
    {
        int GetNumberOfFunctionTypeReferences(int id);
        IEnumerable<FunctionTypeRefData> GetAllAndNotVisibleForCustomer(int customerId);
        IEnumerable<FunctionTypeRefDataListItem> GetFunctionTypeRefData(bool isAdmin, string emailAddress);
        FunctionTypeRefData InsertorUpdate(string typeName);
    }
}
