using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ServiceDeskProcessor : IProcessor
    {
        private readonly Customer _customer;
        private readonly ServiceDesk _serviceDesk;
        private readonly IUnitOfWork _unitOfWork;


        public ServiceDeskProcessor(Customer customer,
            ServiceDesk serviceDesk,
            IUnitOfWork unitOfWork)
        {
            _customer = customer;
            _serviceDesk = serviceDesk;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {

            var newServiceDesk = new ServiceDesk
            {
                Customer = _customer,
                CustomerId = _customer.Id,
                DeskName = _serviceDesk.DeskName,
                ServiceDomains = new List<ServiceDomain>(),
                DeskInputTypes = new List<DeskInputType>(),
                Resolvers = new List<Resolver>(),
                InsertedBy = _serviceDesk.InsertedBy,
                InsertedDate = _serviceDesk.InsertedDate,
                UpdatedBy = _serviceDesk.UpdatedBy,
                UpdatedDate = _serviceDesk.UpdatedDate
            };

            _customer.ServiceDesks.Add(newServiceDesk);
            _unitOfWork.Save();
        }
    }
}