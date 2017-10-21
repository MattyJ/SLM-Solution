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
    public class ResolverGroupTypeRefDataService : IResolverGroupTypeRefDataService
    {
        private readonly IRepository<ResolverGroupTypeRefData> _resolverGroupTypeRefDataRepository;
        private readonly IRepository<Resolver> _resolverRepository;
        private readonly IParameterService _parameterService;
        private readonly IUnitOfWork _unitOfWork;

        public ResolverGroupTypeRefDataService(IRepository<ResolverGroupTypeRefData> resolverGroupTypeRefDataRepository,
            IRepository<Resolver> resolverRepository,
            IParameterService parameterService,
            IUnitOfWork unitOfWork)
        {
            if (resolverGroupTypeRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(resolverGroupTypeRefDataRepository));
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

            _resolverGroupTypeRefDataRepository = resolverGroupTypeRefDataRepository;
            _resolverRepository = resolverRepository;
            _parameterService = parameterService;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<ResolverGroupTypeRefData> All()
        {
            IEnumerable<ResolverGroupTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result =
                    _resolverGroupTypeRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
            });

            return result;
        }

        public ResolverGroupTypeRefData GetById(int id)
        {
            ResolverGroupTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _resolverGroupTypeRefDataRepository.GetById(id);
            });
            return result;
        }

        public int Create(ResolverGroupTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverGroupTypeRefDataRepository.Insert(entity);
                _unitOfWork.Save();
            });
            return entity.Id;
        }

        public void Update(ResolverGroupTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverGroupTypeRefDataRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(ResolverGroupTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverGroupTypeRefDataRepository.Delete(entity);
                _unitOfWork.Save();
            });
        }

        public int GetNumberOfResolverGroupTypeReferences(int id)
        {
            return _resolverRepository.Find(x => x.ResolverGroupType != null && x.ResolverGroupType.Id == id).Count();
        }

        public IEnumerable<ResolverGroupTypeRefData> GetAllAndNotVisibleForCustomer(int customerId)
        {
            List<ResolverGroupTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                // Obtain all the visible resolver group types
                result = _resolverGroupTypeRefDataRepository
                    .Query(x => x.Visible)
                    .ToList();


                // Add all the Non-Visible/Customer Unique Resolver Groups
                result.AddRange(_resolverRepository
                    .Query(x => x.ServiceDesk.CustomerId == customerId && !x.ResolverGroupType.Visible)
                        .Select(x => x.ResolverGroupType).Distinct()
                        .ToList());

                result = result
                    .OrderBy(x => x.SortOrder).ThenBy(x => x.ResolverGroupTypeName)
                    .ToList();
            });

            return result;
        }

        public IEnumerable<ResolverGroupTypeRefDataListItem> GetResolverGroupTypeRefDataWithUsageStats()
        {
            return _resolverGroupTypeRefDataRepository.All().OrderBy(o => o.SortOrder).ThenBy(o => o.ResolverGroupTypeName).Select(r =>
                new ResolverGroupTypeRefDataListItem
                {
                    Id = r.Id,
                    ResolverGroupTypeName = r.ResolverGroupTypeName,
                    Order = r.SortOrder,
                    Visible = r.Visible,
                    UsageCount = GetNumberOfResolverGroupTypeReferences(r.Id)
                }).ToList();
        }

        public ResolverGroupTypeRefData InsertorUpdate(string typeName)
        {
            var resolverGroupType = _resolverGroupTypeRefDataRepository.Query(x => x.ResolverGroupTypeName == typeName.Trim()).FirstOrDefault();
            if (resolverGroupType == null)
            {
                resolverGroupType = new ResolverGroupTypeRefData
                {
                    ResolverGroupTypeName = typeName.Trim(),
                    SortOrder = 5,
                    Visible = false
                };
                _resolverGroupTypeRefDataRepository.Insert(resolverGroupType);
                _unitOfWork.Save();
            }
            else
            {
                var customerSpecificTypeThreshold = _parameterService.GetParameterByNameAndCache<int>(ParameterNames.CustomerSpecificTypeThreshold);
                if (!resolverGroupType.Visible &&
                    GetNumberOfResolverGroupTypeReferences(resolverGroupType.Id) >= customerSpecificTypeThreshold - 1)
                {
                    resolverGroupType.Visible = true;
                    _resolverGroupTypeRefDataRepository.Update(resolverGroupType);
                    _unitOfWork.Save();
                }
            }

            return resolverGroupType;
        }
    }
}