using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ServiceComponentProcessor : IProcessor
    {
        private readonly ServiceDesk _serviceDesk;
        private readonly ServiceFunction _serviceFunction;
        private readonly ServiceComponent _serviceComponent;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceComponentProcessor(ServiceDesk serviceDesk,
            ServiceFunction serviceFunction,
            ServiceComponent serviceComponent,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _serviceDesk = serviceDesk;
            _serviceFunction = serviceFunction;
            _serviceComponent = serviceComponent;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            var newServiceComponent = new ServiceComponent
            {
                ServiceFunctionId = _serviceFunction.Id,
                ServiceFunction = _serviceFunction,
                ComponentName = _serviceComponent.ComponentName,
                ComponentLevel = _serviceComponent.ComponentLevel,
                ServiceActivities = _serviceComponent.ServiceActivities,
                ChildServiceComponents = new List<ServiceComponent>(),
                InsertedBy = _serviceComponent.InsertedBy,
                InsertedDate = _serviceComponent.InsertedDate,
                UpdatedBy = _serviceComponent.UpdatedBy,
                UpdatedDate = _serviceComponent.UpdatedDate
            };

            _serviceFunction.ServiceComponents.Add(newServiceComponent);
            _unitOfWork.Save();

            if (_serviceComponent.Resolver != null)
            {
                var serviceComponentResolverProcessor = new ServiceComponentResolverProcessor(_serviceDesk, _serviceComponent, newServiceComponent, _dataContext, _unitOfWork);
                serviceComponentResolverProcessor.Execute();
            }
            else if (_serviceComponent.ChildServiceComponents != null && _serviceComponent.ChildServiceComponents.Any())
            {
                var childServiceComponentProcessor = new ChildServiceComponentProcessor(_serviceDesk, _serviceFunction, newServiceComponent, _serviceComponent.ChildServiceComponents, _dataContext, _unitOfWork);
                childServiceComponentProcessor.Execute();
            }
        }
    }
}