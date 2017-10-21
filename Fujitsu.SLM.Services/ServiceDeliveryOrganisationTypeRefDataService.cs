using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class ServiceDeliveryOrganisationTypeRefDataService : IServiceDeliveryOrganisationTypeRefDataService
    {
        private readonly IRepository<ServiceDeliveryOrganisationTypeRefData> _serviceDeliveryOrganisationRefDataRepository;
        private readonly IRepository<Resolver> _resolverRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDeliveryOrganisationTypeRefDataService(IRepository<ServiceDeliveryOrganisationTypeRefData> serviceDeliveryOrganisationRefDataRepository,
            IRepository<Resolver> resolverRepository,
            IUnitOfWork unitOfWork)
        {
            if (serviceDeliveryOrganisationRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceDeliveryOrganisationRefDataRepository));
            }

            if (resolverRepository == null)
            {
                throw new ArgumentNullException(nameof(resolverRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _serviceDeliveryOrganisationRefDataRepository = serviceDeliveryOrganisationRefDataRepository;
            _resolverRepository = resolverRepository;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<ServiceDeliveryOrganisationTypeRefData> All()
        {
            IEnumerable<ServiceDeliveryOrganisationTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result =
                    _serviceDeliveryOrganisationRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
            });

            return result;
        }

        public ServiceDeliveryOrganisationTypeRefData GetById(int id)
        {
            ServiceDeliveryOrganisationTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceDeliveryOrganisationRefDataRepository.GetById(id);
            });
            return result;
        }

        public int Create(ServiceDeliveryOrganisationTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceDeliveryOrganisationRefDataRepository.Insert(entity);
                _unitOfWork.Save();
            });
            return entity.Id;
        }

        public void Update(ServiceDeliveryOrganisationTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceDeliveryOrganisationRefDataRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(ServiceDeliveryOrganisationTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceDeliveryOrganisationRefDataRepository.Delete(entity);
                _unitOfWork.Save();
            });
        }

        public bool IsServiceDeliveryOrganisationTypeReferenced(int id)
        {
            var result = false;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _resolverRepository
                    .Any(x => x.ServiceDeliveryOrganisationType != null && x.ServiceDeliveryOrganisationType.Id == id);
            });

            return result;
        }
    }
}