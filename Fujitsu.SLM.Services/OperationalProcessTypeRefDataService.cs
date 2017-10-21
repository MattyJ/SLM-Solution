using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Helpers;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class OperationalProcessTypeRefDataService : IOperationalProcessTypeRefDataService
    {
        private readonly IRepository<OperationalProcessTypeRefData> _operationalProcessRefDataRepository;
        private readonly IRepository<OperationalProcessType> _operationalProcessTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OperationalProcessTypeRefDataService(IRepository<OperationalProcessTypeRefData> operationalProcessRefDataRepository,
            IRepository<OperationalProcessType> operationalProcessTypeRepository,
            IUnitOfWork unitOfWork)
        {
            if (operationalProcessRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(operationalProcessRefDataRepository));
            }

            if (operationalProcessTypeRepository == null)
            {
                throw new ArgumentNullException(nameof(operationalProcessTypeRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _operationalProcessRefDataRepository = operationalProcessRefDataRepository;
            _operationalProcessTypeRepository = operationalProcessTypeRepository;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<OperationalProcessTypeRefData> All()
        {
            IEnumerable<OperationalProcessTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result =
                    _operationalProcessRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
            });

            return result;
        }

        public OperationalProcessTypeRefData GetById(int id)
        {
            OperationalProcessTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _operationalProcessRefDataRepository.GetById(id);
            });
            return result;
        }

        public int Create(OperationalProcessTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _operationalProcessRefDataRepository.Insert(entity);
                _unitOfWork.Save();
            });
            return entity.Id;
        }

        public void Update(OperationalProcessTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _operationalProcessRefDataRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(OperationalProcessTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _operationalProcessRefDataRepository.Delete(entity);
                _unitOfWork.Save();
            });
        }

        public int GetNumberOfOperationalProcessTypeReferences(int id)
        {
            return _operationalProcessTypeRepository.Find(x => x.OperationalProcessTypeRefDataId == id).Count();
        }

        public IEnumerable<OperationalProcessTypeRefData> GetAllAndNotVisibleForCustomer(int customerId)
        {
            List<OperationalProcessTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = OperationalProcessHelper.GetAllAndNotVisibleForCustomer(_operationalProcessRefDataRepository,
                    _operationalProcessTypeRepository,
                    customerId);
            });

            return result;
        }

        public void PurgeOrphans()
        {

            //TODO: MJJ 08/01/2016 Have no idea Ian why this should ever occur, when you remove an operational process it should delete it?
            var nullResolverProcTypes = _operationalProcessTypeRepository.Query(x => x.Resolver == null);
            foreach (var item in nullResolverProcTypes)
            {
                _operationalProcessTypeRepository.Delete(item);
            }
            _unitOfWork.Save();

            // Get a list of all of the utilised operational processes
            // Get a list of non-visible operational processes from ref data and then filter on those that don't exist within the list of all utilised operational processes
            //TODO: MJJ 08/01/2016 Why are we cleaning up here Ian?

            var allProc = _operationalProcessTypeRepository.All().Select(x => x.OperationalProcessTypeRefDataId).Distinct();
            var orphans = _operationalProcessRefDataRepository
                .Query(x => !x.Visible)
                .Where(x => !allProc.Contains(x.Id));
            foreach (var item in orphans)
            {
                _operationalProcessRefDataRepository.Delete(item);
            }
            _unitOfWork.Save();
        }

        public IEnumerable<OperationalProcessTypeRefDataListItem> GetOperationalProcessTypeRefDataWithUsageStats()
        {
            return _operationalProcessRefDataRepository.All().OrderBy(o => o.SortOrder).ThenBy(o => o.OperationalProcessTypeName).Select(o =>
                new OperationalProcessTypeRefDataListItem
                {
                    Id = o.Id,
                    OperationalProcessTypeName = o.OperationalProcessTypeName,
                    SortOrder = o.SortOrder,
                    Visible = o.Visible,
                    Standard = o.Standard,
                    UsageCount = GetNumberOfOperationalProcessTypeReferences(o.Id)
                }).ToList();
        }
    }
}