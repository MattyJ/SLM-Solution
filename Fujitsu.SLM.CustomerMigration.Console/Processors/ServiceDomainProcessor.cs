using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ServiceDomainProcessor : IProcessor
    {
        private readonly ServiceDesk _serviceDesk;
        private readonly IEnumerable<ServiceDomain> _serviceDomains;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;


        public ServiceDomainProcessor(ServiceDesk serviceDesk,
            IEnumerable<ServiceDomain> serviceDomains,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _serviceDesk = serviceDesk;
            _serviceDomains = serviceDomains;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            foreach (var serviceDomain in _serviceDomains)
            {
                var serviceDomainTypeRefData = serviceDomain.DomainType;
                if (!_dataContext.DomainTypeRefData.Any(x => x.DomainName == serviceDomainTypeRefData.DomainName))
                {
                    var domainType = new DomainTypeRefData
                    {
                        DomainName = serviceDomainTypeRefData.DomainName,
                        Visible = serviceDomainTypeRefData.Visible,
                        SortOrder = serviceDomainTypeRefData.SortOrder
                    };

                    _dataContext.DomainTypeRefData.Add(domainType);
                    _unitOfWork.Save();
                }

                var domainTypeRefData = _dataContext.DomainTypeRefData.Single(x => x.DomainName == serviceDomainTypeRefData.DomainName);

                var newServiceDomain = new ServiceDomain
                {
                    ServiceDeskId = _serviceDesk.Id,
                    ServiceDesk = _serviceDesk,
                    DomainType = domainTypeRefData,
                    DomainTypeId = domainTypeRefData.Id,
                    AlternativeName = serviceDomain.AlternativeName,
                    ServiceFunctions = new List<ServiceFunction>(),
                    InsertedBy = serviceDomain.InsertedBy,
                    InsertedDate = serviceDomain.InsertedDate,
                    UpdatedBy = serviceDomain.UpdatedBy,
                    UpdatedDate = serviceDomain.UpdatedDate
                };

                _serviceDesk.ServiceDomains.Add(newServiceDomain);
                _unitOfWork.Save();

                var serviceFunctionProcessor = new ServiceFunctionProcessor(newServiceDomain, serviceDomain.ServiceFunctions.ToList(), _dataContext, _unitOfWork);
                serviceFunctionProcessor.Execute();

                foreach (var serviceFunction in serviceDomain.ServiceFunctions)
                {
                    var targetServiceFunction = newServiceDomain.ServiceFunctions.Single(x => x.FunctionType.FunctionName == serviceFunction.FunctionType.FunctionName);
                    foreach (var serviceComponent in serviceFunction.ServiceComponents.Where(x => x.ComponentLevel == 1))
                    {
                        var serviceComponentProcessor = new ServiceComponentProcessor(_serviceDesk, targetServiceFunction, serviceComponent, _dataContext, _unitOfWork);
                        serviceComponentProcessor.Execute();
                    }
                }
            }
        }
    }
}