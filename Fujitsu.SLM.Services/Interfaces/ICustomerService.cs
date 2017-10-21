using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;

namespace Fujitsu.SLM.Services.Interfaces
{
    public interface ICustomerService : IService<Customer>
    {
        IEnumerable<Customer> MyCustomers(string emailAddress);
        IQueryable<Customer> MyArchives(string emailAddress);
        IQueryable<Customer> Customers();
        IQueryable<Customer> Archives();
        bool IsArchitectACustomerOwner(string emailAddres);
        void Delete(int id);
    }
}
