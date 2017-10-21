using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class ServiceDomainService : IServiceDomainService
    {
        private readonly IRepository<ServiceDomain> _serviceDomainRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDomainService(IRepository<ServiceDomain> serviceDomainRepository, IUnitOfWork unitOfWork)
        {
            if (serviceDomainRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceDomainRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _serviceDomainRepository = serviceDomainRepository;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<ServiceDomain> All()
        {
            IEnumerable<ServiceDomain> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result =
                        _serviceDomainRepository.All().ToList();
                });

            return result;
        }

        public ServiceDomain GetById(int id)
        {
            ServiceDomain result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _serviceDomainRepository.GetById(id);
                });
            return result;
        }

        public ServiceDomain GetByCustomerAndId(int customerId, int id)
        {
            ServiceDomain result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceDomainRepository
                    .SingleOrDefault(x => x.ServiceDesk.CustomerId == customerId && x.Id == id);
            });
            return result;
        }

        public int Create(ServiceDomain entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _serviceDomainRepository.Insert(entity);
                    _unitOfWork.Save();
                });
            return entity.Id;
        }

        public void Update(ServiceDomain entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _serviceDomainRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(ServiceDomain entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    // Get a new sql connection from the unit of work
                    using (var dbConnection = _unitOfWork.CreateConnection())
                    {
                        // Create a null transaction
                        SqlTransaction transaction = null;

                        try
                        {
                            // Open the connection and begin a transaction
                            dbConnection.Open();
                            transaction = dbConnection.BeginTransaction();

                            // Execute the stored procedure to delete the customer
                            _unitOfWork.ExecuteNonQueryStoredProcedure("spDeleteServiceDomain",
                                dbConnection,
                                transaction,
                                new SqlParameter("@ServiceDomainId", SqlDbType.Int) { Value = entity.Id });

                            // Command has executed successfully, commit the transaction
                            transaction.Commit();
                        }
                        catch (Exception)
                        {
                            // If we have a transaction then roll it back
                            transaction?.Rollback();
                            throw;
                        }
                    }
                });
        }
        public IQueryable<ServiceDomainListItem> ServiceDeskDomains(int serviceDeskId)
        {
            IQueryable<ServiceDomainListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _serviceDomainRepository
                        .Query(c => c.ServiceDeskId == serviceDeskId).AsNoTracking().Select(x => new ServiceDomainListItem
                        {
                            Id = x.Id,
                            CustomerName = x.ServiceDesk.Customer.CustomerName,
                            ServiceDeskId = x.ServiceDeskId,
                            ServiceDeskName = x.ServiceDesk.DeskName,
                            DomainTypeId = x.DomainTypeId,
                            DomainName = x.DomainType.DomainName,
                            AlternativeName = x.AlternativeName,
                            DiagramOrder = x.DiagramOrder,
                            UpdatedBy = x.UpdatedBy,
                            UpdatedDate = x.UpdatedDate
                        });
                });

            return result;
        }

        public IQueryable<ServiceDomainListItem> CustomerServiceDomains(int customerId)
        {
            IQueryable<ServiceDomainListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _serviceDomainRepository
                        .Query(c => c.ServiceDesk.CustomerId == customerId).AsNoTracking().Select(x => new ServiceDomainListItem
                        {
                            Id = x.Id,
                            CustomerName = x.ServiceDesk.Customer.CustomerName,
                            ServiceDeskId = x.ServiceDeskId,
                            ServiceDeskName = x.ServiceDesk.DeskName,
                            DomainTypeId = x.DomainTypeId,
                            DomainName = x.DomainType.DomainName,
                            AlternativeName = x.AlternativeName,
                            DiagramOrder = x.DiagramOrder,
                            UpdatedBy = x.UpdatedBy,
                            UpdatedDate = x.UpdatedDate
                        });
                });

            return result;
        }

        public IQueryable<ServiceDomain> GetByCustomer(int customerId)
        {
            IQueryable<ServiceDomain> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceDomainRepository
                    .Query(x => x.ServiceDesk.CustomerId == customerId);
            });

            return result;
        }

    }
}