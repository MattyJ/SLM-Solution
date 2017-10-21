using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IServiceDeliveryUnitTypeRefDataService : IService<ServiceDeliveryUnitTypeRefData>
    {
        IEnumerable<ServiceDeliveryUnitTypeRefData> GetAllAndNotVisibleForCustomer(int customerId);
        bool IsServiceDeliveryUnitTypeReferenced(int id);
        IEnumerable<ServiceDeliveryUnitTypeRefDataListItem> GetServiceDeliveryUnitTypeRefDataWithUsageStats();
        ServiceDeliveryUnitTypeRefData InsertorUpdate(string typeName);
    }
}