using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Contributor> _contributorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IRepository<Customer> customerRepository, IRepository<Contributor> contributorRepository, IUnitOfWork unitOfWork)
        {
            if (customerRepository == null)
            {
                throw new ArgumentNullException(nameof(customerRepository));
            }

            if (contributorRepository == null)
            {
                throw new ArgumentNullException(nameof(contributorRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _customerRepository = customerRepository;
            _contributorRepository = contributorRepository;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Customer> MyCustomers(string emailAddress)
        {
            IList<Customer> myCustomers = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    myCustomers = _customerRepository.Query(c => c.AssignedArchitect == emailAddress && c.Active).AsNoTracking().ToList();

                    foreach (var contributor in _contributorRepository.Query(c => c.EmailAddress == emailAddress && c.Customer.Active).Include("Customer").AsNoTracking().ToList())
                    {
                        if (!myCustomers.Select(s => s.Id).Contains(contributor.CustomerId))
                        {
                            myCustomers.Add(contributor.Customer);
                        }
                    }

                });

            return myCustomers;
        }

        public IQueryable<Customer> Customers()
        {
            IQueryable<Customer> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _customerRepository.Query(c => c.Active).AsNoTracking();
                });

            return result;
        }

        public IQueryable<Customer> MyArchives(string emailAddress)
        {
            IQueryable<Customer> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _customerRepository.Query(c => c.AssignedArchitect == emailAddress && c.Active == false).AsNoTracking();
                });

            return result;
        }

        public IQueryable<Customer> Archives()
        {
            IQueryable<Customer> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _customerRepository.Query(c => c.Active == false).AsNoTracking();
                });

            return result;
        }

        public bool IsArchitectACustomerOwner(string emailAddres)
        {
            var result = false;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _customerRepository
                    .Any(x => x.Active && x.AssignedArchitect == emailAddres);

            });

            return result;
        }
        public IEnumerable<Customer> All()
        {
            IEnumerable<Customer> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    result = _customerRepository.All().ToList();
                });

            return result;
        }

        public Customer GetById(int id)
        {
            Customer result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General,
            () =>
            {
                result = _customerRepository.GetById(id);
            });
            return result;
        }

        public int Create(Customer entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _customerRepository.Insert(entity);
                    _unitOfWork.Save();
                });

            return entity.Id;
        }

        public void Update(Customer entity)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _customerRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(Customer entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _customerRepository.Delete(entity);
                    _unitOfWork.Save();
                });
        }

        public void Update(Customer entity, bool save)
        {

            RetryableOperation.Invoke(ExceptionPolicies.General,
                () =>
                {
                    _customerRepository.Update(entity);
                    _unitOfWork.Save();
                });
        }

        public void Delete(int id)
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
                            _unitOfWork.ExecuteNonQueryStoredProcedure("spDeleteCustomer",
                                dbConnection,
                                transaction,
                                new SqlParameter("@CustomerId", SqlDbType.Int) { Value = id });

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

    }
}
