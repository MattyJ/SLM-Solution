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
    public class ContextHelpRefDataService : IContextHelpRefDataService
    {
        private readonly IRepository<ContextHelpRefData> _contextHelpRefDataRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ContextHelpRefDataService(IRepository<ContextHelpRefData> contextHelpRefDataRepository,
            IUnitOfWork unitOfWork)
        {
            if (contextHelpRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(contextHelpRefDataRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _contextHelpRefDataRepository = contextHelpRefDataRepository;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<ContextHelpRefData> All()
        {
            IEnumerable<ContextHelpRefData> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result =
                        _contextHelpRefDataRepository.All().OrderBy(x => x.Key).ToList();
                });

            return result;
        }

        public ContextHelpRefData GetById(int id)
        {
            ContextHelpRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _contextHelpRefDataRepository.GetById(id);
                });
            return result;
        }

        public int Create(ContextHelpRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _contextHelpRefDataRepository.Insert(entity);
                    _unitOfWork.Save();
                });
            return entity.Id;
        }

        public void Update(ContextHelpRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _contextHelpRefDataRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(ContextHelpRefData entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
                {
                    _contextHelpRefDataRepository.Delete(entity);
                    _unitOfWork.Save();
                });
        }

        public ContextHelpRefData GetByHelpKey(string helpKey)
        {
            ContextHelpRefData result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _contextHelpRefDataRepository.FirstOrDefault(x => x.Key == helpKey);
                });
            return result;
        }
    }
}