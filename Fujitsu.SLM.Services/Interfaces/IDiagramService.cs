using System.Collections.Generic;
using System.Linq;
using Fujitsu.SLM.Model;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IDiagramService : IService<Diagram>
    {
        IQueryable<Diagram> LevelDiagrams(int level, int id);
        IQueryable<Diagram> Diagrams(int id);
        IQueryable<Diagram> GetByCustomerId(int customerId);
        Diagram GetByCustomerAndId(int customerId, int id);
    }
}