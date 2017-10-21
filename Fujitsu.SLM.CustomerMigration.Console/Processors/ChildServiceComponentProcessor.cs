using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ChildServiceComponentProcessor : IProcessor
    {
        private readonly ServiceDesk _serviceDesk;
        private readonly ServiceFunction _serviceFunction;
        private readonly ServiceComponent _parentServiceComponent;
        private readonly IEnumerable<ServiceComponent> _childServiceComponents;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;

        public ChildServiceComponentProcessor(ServiceDesk serviceDesk,
            ServiceFunction serviceFunction,
            ServiceComponent parentServiceComponent,
            IEnumerable<ServiceComponent> childServiceComponents,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _serviceDesk = serviceDesk;
            _serviceFunction = serviceFunction;
            _parentServiceComponent = parentServiceComponent;
            _childServiceComponents = childServiceComponents;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            foreach (var childServiceComponent in _childServiceComponents)
            {
                var newServiceComponent = new ServiceComponent
                {
                    ServiceFunctionId = _serviceFunction.Id,
                    ServiceFunction = _serviceFunction,
                    ParentServiceComponent = _parentServiceComponent,
                    ParentServiceComponentId = _parentServiceComponent.Id,
                    ComponentName = childServiceComponent.ComponentName,
                    ComponentLevel = childServiceComponent.ComponentLevel,
                    ServiceActivities = childServiceComponent.ServiceActivities,
                    InsertedBy = childServiceComponent.InsertedBy,
                    InsertedDate = childServiceComponent.InsertedDate,
                    UpdatedBy = childServiceComponent.UpdatedBy,
                    UpdatedDate = childServiceComponent.UpdatedDate
                };

                _parentServiceComponent.ChildServiceComponents.Add(newServiceComponent);
                _unitOfWork.Save();

                if (childServiceComponent.Resolver != null)
                {
                    var serviceComponentResolverProcessor = new ServiceComponentResolverProcessor(_serviceDesk, childServiceComponent, newServiceComponent, _dataContext, _unitOfWork);
                    serviceComponentResolverProcessor.Execute();
                }
            }
        }
    }
}