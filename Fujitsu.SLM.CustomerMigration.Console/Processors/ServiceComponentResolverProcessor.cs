using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ServiceComponentResolverProcessor : IProcessor
    {
        private readonly ServiceDesk _serviceDesk;
        private readonly ServiceComponent _serviceComponent;
        private readonly ServiceComponent _targetServiceComponent;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceComponentResolverProcessor(ServiceDesk serviceDesk,
            ServiceComponent serviceComponent,
            ServiceComponent targetServiceComponent,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _serviceDesk = serviceDesk;
            _serviceComponent = serviceComponent;
            _targetServiceComponent = targetServiceComponent;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            if (_serviceComponent.Resolver.ServiceDeliveryUnitType != null)
            {
                if (!_dataContext.ServiceDeliveryUnitTypeRefData.Any(x => x.ServiceDeliveryUnitTypeName == _serviceComponent.Resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName))
                {
                    var serviceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData
                    {
                        ServiceDeliveryUnitTypeName = _serviceComponent.Resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName,
                        SortOrder = _serviceComponent.Resolver.ServiceDeliveryUnitType.SortOrder,
                        Visible = _serviceComponent.Resolver.ServiceDeliveryUnitType.Visible
                    };

                    _dataContext.ServiceDeliveryUnitTypeRefData.Add(serviceDeliveryUnitType);
                    _unitOfWork.Save();
                }
            }

            if (_serviceComponent.Resolver.ResolverGroupType != null)
            {
                if (!_dataContext.ResolverGroupTypeRefData.Any(x => x.ResolverGroupTypeName == _serviceComponent.Resolver.ResolverGroupType.ResolverGroupTypeName))
                {
                    var resolverGroupType = new ResolverGroupTypeRefData
                    {
                        ResolverGroupTypeName = _serviceComponent.Resolver.ResolverGroupType.ResolverGroupTypeName,
                        SortOrder = _serviceComponent.Resolver.ResolverGroupType.SortOrder,
                        Visible = _serviceComponent.Resolver.ResolverGroupType.Visible
                    };

                    _dataContext.ResolverGroupTypeRefData.Add(resolverGroupType);
                    _unitOfWork.Save();
                }
            }

            var newResolver = new Resolver
            {
                ServiceDesk = _serviceDesk,
                ServiceDeskId = _serviceDesk.Id,
                ServiceComponent = _targetServiceComponent,
                ServiceDeliveryOrganisationType = _dataContext.ServiceDeliveryOrganisationTypeRefData.Single(x => x.ServiceDeliveryOrganisationTypeName == _serviceComponent.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName),
                ServiceDeliveryOrganisationNotes = _serviceComponent.Resolver.ServiceDeliveryOrganisationNotes,
                ServiceDeliveryUnitNotes = _serviceComponent.Resolver.ServiceDeliveryUnitNotes,
                ServiceDeliveryUnitType = _serviceComponent.Resolver.ServiceDeliveryUnitType != null ? _dataContext.ServiceDeliveryUnitTypeRefData.Single(x => x.ServiceDeliveryUnitTypeName == _serviceComponent.Resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName) : null,
                ResolverGroupType = _serviceComponent.Resolver.ResolverGroupType != null ? _dataContext.ResolverGroupTypeRefData.Single(x => x.ResolverGroupTypeName == _serviceComponent.Resolver.ResolverGroupType.ResolverGroupTypeName) : null,
                OperationalProcessTypes = new List<OperationalProcessType>(),
                InsertedBy = _serviceComponent.Resolver.InsertedBy,
                InsertedDate = _serviceComponent.Resolver.InsertedDate,
                UpdatedBy = _serviceComponent.Resolver.UpdatedBy,
                UpdatedDate = _serviceComponent.Resolver.UpdatedDate
            };


            _targetServiceComponent.Resolver = newResolver;
            _unitOfWork.Save();

            if (_serviceComponent.Resolver.OperationalProcessTypes != null && _serviceComponent.Resolver.OperationalProcessTypes.Any())
            {
                var operationalProcessTypeProcessor = new OperationalProcessTypeProcessor(newResolver, _serviceComponent.Resolver.OperationalProcessTypes, _dataContext, _unitOfWork);
                operationalProcessTypeProcessor.Execute();
            }

        }
    }
}