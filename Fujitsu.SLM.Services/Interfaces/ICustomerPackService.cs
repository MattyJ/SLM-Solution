using Fujitsu.SLM.Model;
using System.Linq;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface ICustomerPackService : IService<CustomerPack>
    {
        IQueryable<CustomerPack> CustomerPacks();
        CustomerPack GetByCustomerAndId(int customerId, int id);
    }
}
