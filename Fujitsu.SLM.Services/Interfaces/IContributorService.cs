using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface IContributorService : IService<Contributor>
    {
        IQueryable<Contributor> GetCustomersContributors(int customerId);
        bool DeleteUserContributors(string userId);
        bool IsContributor(int customerId, string emailAddress);
    }
}
