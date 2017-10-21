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
    public class ServiceFunctionService : IServiceFunctionService
    {
        private readonly IRepository<ServiceFunction> _serviceFunctionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceFunctionService(IRepository<ServiceFunction> serviceFunctionRepository, IUnitOfWork unitOfWork)
        {
            if (serviceFunctionRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceFunctionRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _serviceFunctionRepository = serviceFunctionRepository;
            _unitOfWork = unitOfWork;
        }


        public IEnumerable<ServiceFunction> All()
        {
            IEnumerable<ServiceFunction> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result =
                        _serviceFunctionRepository.All().ToList();
                });

            return result;
        }

        public ServiceFunction GetById(int id)
        {
            ServiceFunction result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          result = _serviceFunctionRepository.GetById(id);
                                      });
            return result;
        }

        public int Create(ServiceFunction entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _serviceFunctionRepository.Insert(entity);
                                          _unitOfWork.Save();
                                      });
            return entity.Id;
        }

        public void Update(ServiceFunction entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                                      () =>
                                      {
                                          _serviceFunctionRepository.Update(entity);
                                          _unitOfWork.Save();
                                      });
        }

        public void Delete(ServiceFunction entity)
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
                            _unitOfWork.ExecuteNonQueryStoredProcedure("spDeleteServiceFunction",
                                dbConnection,
                                transaction,
                                new SqlParameter("@ServiceFunctionId", SqlDbType.Int) { Value = entity.Id });

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

        public IQueryable<ServiceFunctionListItem> ServiceDomainFunctions(int serviceDomainId)
        {
            IQueryable<ServiceFunctionListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _serviceFunctionRepository
                        .Query(c => c.ServiceDomainId == serviceDomainId).AsNoTracking().Select(x => new ServiceFunctionListItem
                        {
                            Id = x.Id,
                            CustomerName = x.ServiceDomain.ServiceDesk.Customer.CustomerName,
                            ServiceDeskName = x.ServiceDomain.ServiceDesk.DeskName,
                            ServiceDomainId = x.ServiceDomainId,
                            ServiceDomainName = x.ServiceDomain.AlternativeName ?? x.ServiceDomain.DomainType.DomainName,
                            FunctionTypeId = x.FunctionTypeId,
                            FunctionName = x.FunctionType.FunctionName,
                            AlternativeName = x.AlternativeName,
                            DiagramOrder = x.DiagramOrder,
                            UpdatedBy = x.UpdatedBy,
                            UpdatedDate = x.UpdatedDate
                        });
                });

            return result;
        }

        public IQueryable<ServiceFunctionListItem> CustomerServiceFunctions(int customerId)
        {
            IQueryable<ServiceFunctionListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _serviceFunctionRepository
                        .Query(c => c.ServiceDomain.ServiceDesk.CustomerId == customerId).AsNoTracking().Select(x => new ServiceFunctionListItem
                        {
                            Id = x.Id,
                            CustomerName = x.ServiceDomain.ServiceDesk.Customer.CustomerName,
                            ServiceDeskName = x.ServiceDomain.ServiceDesk.DeskName,
                            ServiceDomainId = x.ServiceDomainId,
                            ServiceDomainName = x.ServiceDomain.AlternativeName ?? x.ServiceDomain.DomainType.DomainName,
                            FunctionTypeId = x.FunctionTypeId,
                            FunctionName = x.FunctionType.FunctionName,
                            AlternativeName = x.AlternativeName,
                            DiagramOrder = x.DiagramOrder,
                            UpdatedBy = x.UpdatedBy,
                            UpdatedDate = x.UpdatedDate
                        });
                });

            return result;
        }
    }
}
