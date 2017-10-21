using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IServiceDeskService : IService<ServiceDesk>
    {
        void Update(ServiceDesk entity, List<int> inputTypeDeletions);
        void Update(IEnumerable<ServiceDesk> entities);
        IQueryable<ServiceDesk> GetByCustomer(int customerId);
        ServiceDesk GetByCustomerAndId(int customerId, int id);
        void Clear(int serviceDeskId);
    }
}