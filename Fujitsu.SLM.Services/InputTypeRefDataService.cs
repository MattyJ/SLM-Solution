using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class InputTypeRefDataService : IInputTypeRefDataService
    {
        private readonly IRepository<InputTypeRefData> _inputTypeRefDataRepository;
        private readonly IRepository<DeskInputType> _deskInputTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InputTypeRefDataService(IRepository<InputTypeRefData> inputTypeRefDataRepository,
            IRepository<DeskInputType> deskInputTypeRepository, IUnitOfWork unitOfWork)
        {
            if (inputTypeRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(inputTypeRefDataRepository));
            }

            if (deskInputTypeRepository == null)
            {
                throw new ArgumentNullException(nameof(deskInputTypeRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _inputTypeRefDataRepository = inputTypeRefDataRepository;
            _deskInputTypeRepository = deskInputTypeRepository;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<InputTypeRefData> All()
        {
            IEnumerable<InputTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result =
                        _inputTypeRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
                });

            return result;
        }

        public InputTypeRefData GetById(int id)
        {
            InputTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _inputTypeRefDataRepository.GetById(id);
                                      });
            return result;
        }

        public int Create(InputTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _inputTypeRefDataRepository.Insert(entity);
                                          _unitOfWork.Save();
                                      });
            return entity.Id;
        }

        public void Update(InputTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _inputTypeRefDataRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(InputTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _inputTypeRefDataRepository.Delete(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public bool IsInputTypeReferenced(int id)
        {
            return _deskInputTypeRepository.Any(x => x.InputTypeRefData.Id == id);
        }

        public IEnumerable<InputTypeRefDataListItem> GetInputTypeRefDataWithUsageStats()
        {
            {
                return _inputTypeRefDataRepository.All().OrderBy(o => o.SortOrder).ThenBy(o => o.InputTypeNumber).Select(i =>
                new InputTypeRefDataListItem
                {
                    Id = i.Id,
                    InputTypeNumber = i.InputTypeNumber,
                    InputTypeName = i.InputTypeName,
                    Default = i.Default,
                    SortOrder = i.SortOrder,
                    UsageCount = _deskInputTypeRepository.All().Count(x => x.InputTypeRefData.Id == i.Id)
                }).ToList();
            }
        }
    }
}
