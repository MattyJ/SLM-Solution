using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class ServiceDeskService : IServiceDeskService
    {
        private readonly IRepository<ServiceDesk> _serviceDeskRepository;
        private readonly IRepository<DeskInputType> _deskInputTypeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDeskService(IRepository<ServiceDesk> serviceDeskRepository,
            IRepository<DeskInputType> deskInputTypeRepository,
            IUnitOfWork unitOfWork)
        {
            if (serviceDeskRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskRepository));
            }

            if (deskInputTypeRepository == null)
            {
                throw new ArgumentNullException(nameof(deskInputTypeRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _serviceDeskRepository = serviceDeskRepository;
            _unitOfWork = unitOfWork;
            _deskInputTypeRepository = deskInputTypeRepository;
        }


        public IEnumerable<ServiceDesk> All()
        {
            IEnumerable<ServiceDesk> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _serviceDeskRepository.All().ToList();
                });

            return result;
        }

        public ServiceDesk GetById(int id)
        {
            ServiceDesk result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _serviceDeskRepository.GetById(id);
                });
            return result;
        }

        public IQueryable<ServiceDesk> GetByCustomer(int customerId)
        {
            IQueryable<ServiceDesk> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceDeskRepository
                    .Query(x => x.CustomerId == customerId);
            });

            return result;
        }

        public ServiceDesk GetByCustomerAndId(int customerId, int id)
        {
            ServiceDesk result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceDeskRepository.SingleOrDefault(x => x.CustomerId == customerId && x.Id == id);
            });
            return result;
        }

        public int Create(ServiceDesk entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _serviceDeskRepository.Insert(entity);
                    _unitOfWork.Save();
                });
            return entity.Id;
        }

        public void Update(ServiceDesk entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _serviceDeskRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Update(ServiceDesk entity, List<int> inputTypeDeletions)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    foreach (var inputType in inputTypeDeletions)
                    {
                        _deskInputTypeRepository.Delete(inputType);
                    }

                    _serviceDeskRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(ServiceDesk entity)
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
                            _unitOfWork.ExecuteNonQueryStoredProcedure("spDeleteServiceDesk",
                                dbConnection,
                                transaction,
                                new SqlParameter("@ServiceDeskId", SqlDbType.Int) { Value = entity.Id });

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

        public void Update(IEnumerable<ServiceDesk> entities)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                if (entities != null)
                {
                    foreach (var entity in entities)
                    {
                        _serviceDeskRepository.Update(entity);
                    }
                }
                _unitOfWork.Save();
            });
        }

        public void Clear(int serviceDeskId)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                object[] parameters = { new SqlParameter { ParameterName = "ServiceDeskId", DbType = DbType.Int32, Value = serviceDeskId } };
                _serviceDeskRepository.ExecuteSqlCommand("EXECUTE [dbo].[spDeleteServiceDeskContents] @ServiceDeskId", parameters);
                _unitOfWork.Save();
            });
        }
    }

}