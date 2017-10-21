using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class OperationalProcessTypeProcessor : IProcessor
    {
        private readonly Resolver _resolver;
        private readonly IEnumerable<OperationalProcessType> _operationalProcessTypes;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;


        public OperationalProcessTypeProcessor(Resolver resolver,
            IEnumerable<OperationalProcessType> operationalProcessTypes,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _resolver = resolver;
            _operationalProcessTypes = operationalProcessTypes;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            foreach (var operationalProcess in _operationalProcessTypes)
            {
                var operationalProcessTypeRefData = operationalProcess.OperationalProcessTypeRefData;
                if (!_dataContext.OperationalProcessTypeRefData.Any(x => x.OperationalProcessTypeName == operationalProcessTypeRefData.OperationalProcessTypeName))
                {
                    var operationalProcessType = new OperationalProcessTypeRefData
                    {
                        OperationalProcessTypeName = operationalProcessTypeRefData.OperationalProcessTypeName,
                        Visible = operationalProcessTypeRefData.Visible,
                        SortOrder = operationalProcessTypeRefData.SortOrder
                    };

                    _dataContext.OperationalProcessTypeRefData.Add(operationalProcessType);
                    _unitOfWork.Save();
                }

                var proccessType = new OperationalProcessType
                {
                    Resolver = _resolver,
                    OperationalProcessTypeRefData = _dataContext.OperationalProcessTypeRefData.Single(x => x.OperationalProcessTypeName == operationalProcessTypeRefData.OperationalProcessTypeName)
                };

                _resolver.OperationalProcessTypes.Add(proccessType);
                _unitOfWork.Save();
            }
        }
    }
}