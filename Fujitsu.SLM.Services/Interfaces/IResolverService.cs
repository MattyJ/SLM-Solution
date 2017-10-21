using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IResolverService : IService<Resolver>
    {
        List<List<DotMatrixListItem>> GetDotMatrix(int customerId, bool standard);
        List<List<DotMatrixListItem>> GetDotMatrix(int customerId, bool standard, int? serviceDeskId);
        IQueryable<Resolver> GetByCustomer(int customerId);
        IQueryable<ResolverListItem> GetListByCustomer(int customerId);
        void Update(IEnumerable<Resolver> entities);
        void Create(Resolver resolver, bool save);
        void Update(Resolver resolver, bool save);
        void Delete(Resolver resolver, bool save);
        void Move(int resolverId, int destinationDeskId);
    }
}