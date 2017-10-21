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
    public class DomainTypeRefDataService : IDomainTypeRefDataService
    {
        private readonly IRepository<DomainTypeRefData> _domainTypeRefDataRepository;
        private readonly IRepository<ServiceDesk> _serviceDeskRepository;
        private readonly IRepository<ServiceDomain> _serviceDomainRepository;
        private readonly IParameterService _parameterService;
        private readonly IUnitOfWork _unitOfWork;

        public DomainTypeRefDataService(IRepository<DomainTypeRefData> domainTypeRefDataRepository,
            IRepository<ServiceDesk> serviceDeskRepository,
            IRepository<ServiceDomain> serviceDomainRepository,
            IParameterService parameterService,
            IUnitOfWork unitOfWork)
        {
            if (domainTypeRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(domainTypeRefDataRepository));
            }

            if (serviceDeskRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskRepository));
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

            _domainTypeRefDataRepository = domainTypeRefDataRepository;
            _serviceDeskRepository = serviceDeskRepository;
            _serviceDomainRepository = serviceDomainRepository;
            _parameterService = parameterService;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<DomainTypeRefData> All()
        {
            IEnumerable<DomainTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result =
                        _domainTypeRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
                });

            return result;
        }

        public DomainTypeRefData GetById(int id)
        {
            DomainTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _domainTypeRefDataRepository.GetById(id);
                });
            return result;
        }

        public int Create(DomainTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _domainTypeRefDataRepository.Insert(entity);
                    _unitOfWork.Save();
                });
            return entity.Id;
        }

        public void Update(DomainTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _domainTypeRefDataRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(DomainTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
                {
                    _domainTypeRefDataRepository.Delete(entity);
                    _unitOfWork.Save();
                });
        }

        public IEnumerable<DomainTypeRefData> GetAllAndNotVisibleForCustomer(int customerId)
        {
            List<DomainTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _domainTypeRefDataRepository
                    .Query(x => x.Visible)
                    .ToList();

                result.AddRange(_serviceDeskRepository
                    .Query(w => w.CustomerId == customerId)
                    .SelectMany(s => s.ServiceDomains
                        .Select(sd => sd.DomainType)
                        .Where(f => !f.Visible))
                    .ToList());

                result = result
                    .OrderBy(o => o.SortOrder).ThenBy(o => o.DomainName)
                    .ToList();
            });

            return result;
        }

        public int GetNumberOfDomainTypeReferences(int id)
        {
            return _serviceDomainRepository.Find(x => x.DomainTypeId == id).Count();
        }

        public IEnumerable<DomainTypeRefDataListItem> GetDomainTypeRefData(bool isAdmin, string emailAddress)
        {
            var result = _domainTypeRefDataRepository.All().OrderBy(o => o.SortOrder).ThenBy(o => o.DomainName).Select(d =>
                new DomainTypeRefDataListItem
                {
                    Id = d.Id,
                    DomainName = d.DomainName,
                    SortOrder = d.SortOrder,
                    Visible = d.Visible,
                    UsageCount = GetNumberOfDomainTypeReferences(d.Id),
                    CanEdit = isAdmin,
                    CanDelete = isAdmin
                }).ToList();

            if (isAdmin || string.IsNullOrEmpty(emailAddress)) return result;

            foreach (var domainType in result.Where(d => !d.Visible && d.UsageCount == 1))
            {
                var domain = _serviceDomainRepository.FirstOrDefault(y => y.DomainTypeId == domainType.Id);
                if (domain.ServiceDesk.Customer.AssignedArchitect.SafeEquals(emailAddress) ||
                    domain.ServiceDesk.Customer.Contributors.Any(x => x.EmailAddress == emailAddress))
                {
                    domainType.CanEdit = true;
                }
            }

            return result;
        }

        public DomainTypeRefData InsertorUpdate(string typeName)
        {
            var domainType = _domainTypeRefDataRepository.Query(x => x.DomainName == typeName.Trim()).FirstOrDefault();
            if (domainType == null)
            {
                domainType = new DomainTypeRefData
                {
                    DomainName = typeName.Trim(),
                    SortOrder = 5,
                    Visible = false
                };

                _domainTypeRefDataRepository.Insert(domainType);
                _unitOfWork.Save();
            }
            else
            {
                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                if (!domainType.Visible &&
                    GetNumberOfDomainTypeReferences(domainType.Id) >= customerSpecificTypeThreshold - 1)
                {
                    domainType.Visible = true;
                    _domainTypeRefDataRepository.Update(domainType);
                    _unitOfWork.Save();
                }
            }

            return domainType;
        }
    }
}