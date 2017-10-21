using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ServiceFunctionProcessor : IProcessor
    {
        private readonly ServiceDomain _serviceDomain;
        private readonly IEnumerable<ServiceFunction> _serviceFunctions;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceFunctionProcessor(ServiceDomain serviceDomain,
            IEnumerable<ServiceFunction> serviceFunctions,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _serviceDomain = serviceDomain;
            _serviceFunctions = serviceFunctions;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            foreach (var serviceFunction in _serviceFunctions)
            {
                var serviceFunctionTypeRefData = serviceFunction.FunctionType;
                if (!_dataContext.FunctionTypeRefData.Any(x => x.FunctionName == serviceFunctionTypeRefData.FunctionName))
                {
                    var functionType = new FunctionTypeRefData
                    {
                        FunctionName = serviceFunctionTypeRefData.FunctionName,
                        Visible = serviceFunctionTypeRefData.Visible,
                        SortOrder = serviceFunctionTypeRefData.SortOrder
                    };

                    _dataContext.FunctionTypeRefData.Add(functionType);
                    _unitOfWork.Save();
                }

                var functionTypeRefData = _dataContext.FunctionTypeRefData.Single(x => x.FunctionName == serviceFunctionTypeRefData.FunctionName);

                var newServiceFunction = new ServiceFunction
                {
                    ServiceDomainId = _serviceDomain.Id,
                    ServiceDomain = _serviceDomain,
                    FunctionType = functionTypeRefData,
                    FunctionTypeId = functionTypeRefData.Id,
                    AlternativeName = serviceFunction.AlternativeName,
                    ServiceComponents = new List<ServiceComponent>(),
                    InsertedBy = serviceFunction.InsertedBy,
                    InsertedDate = serviceFunction.InsertedDate,
                    UpdatedBy = serviceFunction.UpdatedBy,
                    UpdatedDate = serviceFunction.UpdatedDate
                };

                _serviceDomain.ServiceFunctions.Add(newServiceFunction);
                _unitOfWork.Save();
            }
        }
    }
}