using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class FunctionTypeRefDataService : IFunctionTypeRefDataService
    {
        private readonly IRepository<FunctionTypeRefData> _functionTypeRefDataRepository;
        private readonly IRepository<ServiceFunction> _serviceFunctionRepository;
        private readonly IRepository<ServiceDomain> _serviceDomainRepository;
        private readonly IParameterService _parameterService;
        private readonly IUnitOfWork _unitOfWork;

        public FunctionTypeRefDataService(IRepository<FunctionTypeRefData> functionTypeRefDataRepository,
            IRepository<ServiceFunction> serviceFunctionRepository,
            IRepository<ServiceDomain> serviceDomainRepository,
            IParameterService parameterService,
            IUnitOfWork unitOfWork)
        {
            if (functionTypeRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(functionTypeRefDataRepository));
            }

            if (serviceFunctionRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceFunctionRepository));
            }

            if (serviceDomainRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceDomainRepository));
            }

            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }
            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _functionTypeRefDataRepository = functionTypeRefDataRepository;
            _serviceFunctionRepository = serviceFunctionRepository;
            _serviceDomainRepository = serviceDomainRepository;
            _parameterService = parameterService;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<FunctionTypeRefData> All()
        {
            IEnumerable<FunctionTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result =
                        _functionTypeRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
                });

            return result;
        }

        public FunctionTypeRefData GetById(int id)
        {
            FunctionTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _functionTypeRefDataRepository.GetById(id);
                                      });
            return result;
        }

        public int Create(FunctionTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _functionTypeRefDataRepository.Insert(entity);
                                          _unitOfWork.Save();
                                      });
            return entity.Id;
        }

        public void Update(FunctionTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _functionTypeRefDataRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(FunctionTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _functionTypeRefDataRepository.Delete(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public int GetNumberOfFunctionTypeReferences(int id)
        {
            return _serviceFunctionRepository.Find(x => x.FunctionTypeId == id).Count();
        }

        public IEnumerable<FunctionTypeRefData> GetAllAndNotVisibleForCustomer(int customerId)
        {
            List<FunctionTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _functionTypeRefDataRepository
                    .Query(x => x.Visible)
                    .ToList();

                result.AddRange(_serviceDomainRepository
                    .Query(w => w.ServiceDesk.CustomerId == customerId)
                    .SelectMany(s => s.ServiceFunctions
                        .Select(sd => sd.FunctionType)
                        .Where(f => !f.Visible))
                    .ToList());

                result = result
                    .OrderBy(o => o.SortOrder).ThenBy(o => o.FunctionName)
                    .ToList();
            });

            return result;
        }

        public IEnumerable<FunctionTypeRefDataListItem> GetFunctionTypeRefData(bool isAdmin, string emailAddress)
        {
            var result = _functionTypeRefDataRepository.All().OrderBy(o => o.SortOrder).ThenBy(o => o.FunctionName).Select(d =>
                new FunctionTypeRefDataListItem
                {
                    Id = d.Id,
                    FunctionName = d.FunctionName,
                    SortOrder = d.SortOrder,
                    Visible = d.Visible,
                    UsageCount = GetNumberOfFunctionTypeReferences(d.Id),
                    CanEdit = isAdmin,
                    CanDelete = isAdmin
                }).ToList();

            if (isAdmin || string.IsNullOrEmpty(emailAddress)) return result;

            foreach (var functionType in result.Where(d => !d.Visible && d.UsageCount == 1))
            {
                var function = _serviceFunctionRepository.FirstOrDefault(y => y.FunctionTypeId == functionType.Id);
                if (function.ServiceDomain.ServiceDesk.Customer.AssignedArchitect.SafeEquals(emailAddress) ||
                    function.ServiceDomain.ServiceDesk.Customer.Contributors.Any(x => x.EmailAddress == emailAddress))
                {
                    functionType.CanEdit = true;
                }
            }

            return result;
        }

        public FunctionTypeRefData InsertorUpdate(string typeName)
        {
            var functionType = _functionTypeRefDataRepository.Query(x => x.FunctionName == typeName.Trim()).FirstOrDefault();
            if (functionType == null)
            {
                functionType = new FunctionTypeRefData
                {
                    FunctionName = typeName.Trim(),
                    SortOrder = 5,
                    Visible = false
                };

                _functionTypeRefDataRepository.Insert(functionType);
                _unitOfWork.Save();
            }
            else
            {
                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                if (!functionType.Visible &&
                    GetNumberOfFunctionTypeReferences(functionType.Id) >= customerSpecificTypeThreshold - 1)
                {
                    functionType.Visible = true;
                    _functionTypeRefDataRepository.Update(functionType);
                    _unitOfWork.Save();
                }
            }

            return functionType;
        }
    }
}
