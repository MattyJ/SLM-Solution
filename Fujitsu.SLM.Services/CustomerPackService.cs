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
    public class CustomerPackService : ICustomerPackService
    {
        private readonly IRepository<CustomerPack> _customerPackRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerPackService(IRepository<CustomerPack> customerPackRepository, IUnitOfWork unitOfWork)
        {
            if (customerPackRepository == null)
            {
                throw new ArgumentNullException(nameof(customerPackRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _customerPackRepository = customerPackRepository;
            _unitOfWork = unitOfWork;
        }

        public IQueryable<CustomerPack> CustomerPacks()
        {
            IQueryable<CustomerPack> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _customerPackRepository.Query(q => true);
            });

            return result;
        }

        public CustomerPack GetById(int id)
        {
            CustomerPack result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _customerPackRepository.GetById(id);
            });
            return result;
        }

        public CustomerPack GetByCustomerAndId(int customerId, int id)
        {
            CustomerPack result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _customerPackRepository
                    .Query(q => q.CustomerId == customerId && q.Id == id)
                    .SingleOrDefault();
            });
            return result;
        }

        public IEnumerable<CustomerPack> All()
        {
            IEnumerable<CustomerPack> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _customerPackRepository.All().ToList();
            });

            return result;
        }

        public int Create(CustomerPack entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _customerPackRepository.Insert(entity);
                _unitOfWork.Save();
            });

            return entity.Id;
        }

        public void Update(CustomerPack entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _customerPackRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(CustomerPack entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _customerPackRepository.Delete(entity);
                _unitOfWork.Save();
            });
        }
    }
}