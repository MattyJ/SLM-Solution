using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Linq;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IServiceDomainService : IService<ServiceDomain>
    {
        IQueryable<ServiceDomainListItem> ServiceDeskDomains(int serviceDeskId);
        IQueryable<ServiceDomainListItem> CustomerServiceDomains(int customerId);
        ServiceDomain GetByCustomerAndId(int customerId, int id);
        IQueryable<ServiceDomain> GetByCustomer(int customerId);
    }
}