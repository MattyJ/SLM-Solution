using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class AuditService : IAuditService
    {
        private readonly IRepository<Audit> _auditRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;

        public AuditService(IRepository<Audit> auditRepository,
            IRepository<Customer> customerRepository,
            IUserIdentity userIdentity,
            IUnitOfWork unitOfWork)
        {
            if (auditRepository == null)
            {
                throw new ArgumentNullException(nameof(auditRepository));
            }

            if (customerRepository == null)
            {
                throw new ArgumentNullException(nameof(customerRepository));
            }

            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _auditRepository = auditRepository;
            _customerRepository = customerRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        public Audit GetById(int id)
        {
            Audit result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _auditRepository.GetById(id);
            });
            return result;
        }

        public IEnumerable<Audit> All()
        {
            IEnumerable<Audit> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _auditRepository.All().ToList();
            });

            return result;
        }

        public int Create(Audit entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _auditRepository.Insert(entity);
                _unitOfWork.Save();
            });

            return entity.Id;
        }

        public void Update(Audit entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _auditRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Delete(Audit entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _auditRepository.Delete(entity);
                _unitOfWork.Save();
            });
        }

        public void CreateAuditBaseline(Customer customer)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                var userName = _userIdentity.Name;
                var now = DateTime.Now;

                customer.Baseline = true;
                customer.UpdatedDate = now;
                customer.UpdatedBy = userName;
                _customerRepository.Update(customer);

                var audit = new Audit
                {
                    Customer = customer,
                    CustomerId = customer.Id,
                    ReasonForIssue = "Initial Customer Baseline Version",
                    Version = 1.0,
                    InsertedBy = userName,
                    InsertedDate = now,
                    UpdatedBy = userName,
                    UpdatedDate = now
                };

                _auditRepository.Insert(audit);
                _unitOfWork.Save();
            });
        }

        public IEnumerable<Audit> CustomerAudits(int customerId)
        {
            IList<Audit> customerAudits = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    customerAudits = _auditRepository.Query(c => c.CustomerId == customerId).AsNoTracking().ToList();

                });

            return customerAudits;
        }
    }
}