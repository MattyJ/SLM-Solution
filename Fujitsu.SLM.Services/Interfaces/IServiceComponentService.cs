using System.Collections.Generic;
using System.Linq;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IServiceComponentService : IService<ServiceComponent>
    {
        IQueryable<ServiceComponent> GetByCustomer(int customerId);
        IQueryable<ServiceComponentListItem> GetByCustomerWithHierarchy(int customerId);
        IQueryable<ResolverListItem> GetResolverByCustomerWithHierarchy(int customerId);
        List<ServiceOrganisationListItem> GetServiceOrganisationResolversByDesk(int serviceDeskId, string organisationType);
        void Create(IEnumerable<ServiceComponent> entities);
        void Update(IEnumerable<ServiceComponent> entities);
        void MoveResolver(int customerId, int sourceServiceComponentId, int destinationServiceComponentId);
    }
}