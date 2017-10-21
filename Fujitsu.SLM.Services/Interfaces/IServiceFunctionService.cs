using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IServiceFunctionService : IService<ServiceFunction>
    {
        IQueryable<ServiceFunctionListItem> ServiceDomainFunctions(int serviceDomainId);
        IQueryable<ServiceFunctionListItem> CustomerServiceFunctions(int customerId);
    }
}
