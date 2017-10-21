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
    public class ServiceDeliveryUnitTypeRefDataService : IServiceDeliveryUnitTypeRefDataService
    {
        private readonly IRepository<ServiceDeliveryUnitTypeRefData> _serviceDeliveryUnitTypeRefDataRepository;
        private readonly IRepository<Resolver> _resolverRepository;
        private readonly IParameterService _parameterService;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDeliveryUnitTypeRefDataService(IRepository<ServiceDeliveryUnitTypeRefData> serviceDeliveryUnitTypeRefDataRepository,
            IRepository<Resolver> resolverRepository,
            IParameterService parameterService,
            IUnitOfWork unitOfWork)
        {
            if (serviceDeliveryUnitTypeRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceDeliveryUnitTypeRefDataRepository));
            }

            if (resolverRepository == null)
            {
                throw new ArgumentNullException(nameof(resolverRepository));
            }

            if (parameterService == null)
            {
                throw new ArgumentNullException(nameof(parameterService));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _serviceDeliveryUnitTypeRefDataRepository = serviceDeliveryUnitTypeRefDataRepository;
            _resolverRepository = resolverRepository;
            _parameterService = parameterService;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<ServiceDeliveryUnitTypeRefData> All()
        {
            IEnumerable<ServiceDeliveryUnitTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result =
                    _serviceDeliveryUnitTypeRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
            });

            return result;
        }

        public ServiceDeliveryUnitTypeRefData GetById(int id)
        {
            ServiceDeliveryUnitTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceDeliveryUnitTypeRefDataRepository.GetById(id);
            });
            return result;
        }

        public int Create(ServiceDeliveryUnitTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceDeliveryUnitTypeRefDataRepository.Insert(entity);
                _unitOfWork.Save();
            });
            return entity.Id;
        }

        public void Update(ServiceDeliveryUnitTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceDeliveryUnitTypeRefDataRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(ServiceDeliveryUnitTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceDeliveryUnitTypeRefDataRepository.Delete(entity);
                _unitOfWork.Save();
            });
        }

        public bool IsServiceDeliveryUnitTypeReferenced(int id)
        {
            var result = false;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _resolverRepository
                    .Any(x => x.ServiceDeliveryUnitType != null && x.ServiceDeliveryUnitType.Id == id);
            });

            return result;
        }

        public IEnumerable<ServiceDeliveryUnitTypeRefDataListItem> GetServiceDeliveryUnitTypeRefDataWithUsageStats()
        {
            return _serviceDeliveryUnitTypeRefDataRepository.All().OrderBy(o => o.SortOrder).ThenBy(o => o.ServiceDeliveryUnitTypeName).Select(s =>
            new ServiceDeliveryUnitTypeRefDataListItem
            {
                Id = s.Id,
                ServiceDeliveryUnitTypeName = s.ServiceDeliveryUnitTypeName,
                SortOrder = s.SortOrder,
                Visible = s.Visible,
                UsageCount = _resolverRepository.All().Count(x => x.ServiceDeliveryUnitType != null && x.ServiceDeliveryUnitType.Id == s.Id)
            }).ToList();
        }

        public IEnumerable<ServiceDeliveryUnitTypeRefData> GetAllAndNotVisibleForCustomer(int customerId)
        {
            List<ServiceDeliveryUnitTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                // Obtain all the visible resolver group types
                result = _serviceDeliveryUnitTypeRefDataRepository
                    .Query(x => x.Visible)
                    .ToList();


                // Add all the Non-Visible/Customer Unique Resolver Groups
                result.AddRange(_resolverRepository
                    .Query(x => x.ServiceDesk.CustomerId == customerId && !x.ServiceDeliveryUnitType.Visible)
                        .Select(x => x.ServiceDeliveryUnitType).Distinct()
                        .ToList());

                result = result
                    .OrderBy(x => x.SortOrder).ThenBy(x => x.ServiceDeliveryUnitTypeName)
                    .ToList();
            });

            return result;
        }

        public ServiceDeliveryUnitTypeRefData InsertorUpdate(string typeName)
        {
            var serviceDeliveryUnitType = _serviceDeliveryUnitTypeRefDataRepository.Query(x => x.ServiceDeliveryUnitTypeName == typeName.Trim()).FirstOrDefault();
            if (serviceDeliveryUnitType == null)
            {
                serviceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData
                {
                    ServiceDeliveryUnitTypeName = typeName.Trim(),
                    SortOrder = 5,
                    Visible = false
                };

                _serviceDeliveryUnitTypeRefDataRepository.Insert(serviceDeliveryUnitType);
                _unitOfWork.Save();
            }
            else
            {
                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                if (!serviceDeliveryUnitType.Visible &&
                    GetNumberServiceDeliveryUnitTypeReferences(serviceDeliveryUnitType.Id) >= customerSpecificTypeThreshold - 1)
                {
                    serviceDeliveryUnitType.Visible = true;
                    _serviceDeliveryUnitTypeRefDataRepository.Update(serviceDeliveryUnitType);
                    _unitOfWork.Save();
                }
            }

            return serviceDeliveryUnitType;
        }

        private int GetNumberServiceDeliveryUnitTypeReferences(int id)
        {
            return _resolverRepository.Find(x => x.ServiceDeliveryUnitType != null && x.ServiceDeliveryUnitType.Id == id).Count();
        }
    }
}