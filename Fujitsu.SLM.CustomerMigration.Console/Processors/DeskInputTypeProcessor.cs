using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class DeskInputTypeProcessor : IProcessor
    {
        private readonly ServiceDesk _serviceDesk;
        private readonly IEnumerable<DeskInputType> _deskInputTypes;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;


        public DeskInputTypeProcessor(ServiceDesk serviceDesk,
            IEnumerable<DeskInputType> deskInputTypes,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _serviceDesk = serviceDesk;
            _deskInputTypes = deskInputTypes;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            foreach (var deskInput in _deskInputTypes)
            {
                var deskInputTypeRefData = deskInput.InputTypeRefData;
                if (!_dataContext.InputTypeRefData.Any(x => x.InputTypeName == deskInputTypeRefData.InputTypeName))
                {
                    var inputType = new InputTypeRefData
                    {
                        InputTypeName = deskInputTypeRefData.InputTypeName,
                        InputTypeNumber = deskInputTypeRefData.InputTypeNumber,
                        SortOrder = deskInputTypeRefData.SortOrder
                    };

                    _dataContext.InputTypeRefData.Add(inputType);
                    _unitOfWork.Save();
                }

                var deskInputType = new DeskInputType
                {
                    ServiceDeskId = _serviceDesk.Id,
                    ServiceDesk = _serviceDesk,
                    InputTypeRefData = _dataContext.InputTypeRefData.Single(x => x.InputTypeName == deskInputTypeRefData.InputTypeName)
                };

                _serviceDesk.DeskInputTypes.Add(deskInputType);
                _unitOfWork.Save();
            }
        }
    }
}