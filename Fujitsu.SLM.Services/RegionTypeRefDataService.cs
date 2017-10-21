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
    public class RegionTypeRefDataService : IRegionTypeRefDataService
    {
        private readonly IRepository<RegionTypeRefData> _regionTypeRefDataRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RegionTypeRefDataService(IRepository<RegionTypeRefData> regionTypeRefDataRepository,
            IUnitOfWork unitOfWork)
        {
            if (regionTypeRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(regionTypeRefDataRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _regionTypeRefDataRepository = regionTypeRefDataRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<RegionTypeRefData> All()
        {
            IEnumerable<RegionTypeRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _regionTypeRefDataRepository.All().OrderBy(x => x.SortOrder).ToList();
                });

            return result;
        }

        public RegionTypeRefData GetById(int id)
        {
            RegionTypeRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _regionTypeRefDataRepository.GetById(id);
                });
            return result;
        }

        public int Create(RegionTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _regionTypeRefDataRepository.Insert(entity);
                    _unitOfWork.Save();
                });
            return entity.Id;
        }

        public void Update(RegionTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _regionTypeRefDataRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(RegionTypeRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
                {
                    _regionTypeRefDataRepository.Delete(entity);
                    _unitOfWork.Save();
                });
        }
    }
}