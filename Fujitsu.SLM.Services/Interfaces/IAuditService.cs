using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IAuditService : IService<Audit>
    {
        void CreateAuditBaseline(Customer customer);
        IEnumerable<Audit> CustomerAudits(int customerId);
    }
}