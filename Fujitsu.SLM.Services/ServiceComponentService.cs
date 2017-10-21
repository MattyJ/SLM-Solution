using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Interfaces;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace Fujitsu.SLM.Services
{
    public class ServiceComponentService : IServiceComponentService
    {
        private readonly IRepository<ServiceComponent> _serviceComponentRepository;
        private readonly IRepository<Resolver> _resolverRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserIdentity _userIdentity;

        private const string NoneSpecified = "None Specified";

        public ServiceComponentService(IRepository<ServiceComponent> serviceComponentRepository,
            IRepository<Resolver> resolverRepository,
            IUnitOfWork unitOfWork,
            IUserIdentity userIdentity)
        {
            if (serviceComponentRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentRepository));
            }

            if (resolverRepository == null)
            {
                throw new ArgumentNullException(nameof(resolverRepository));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            _serviceComponentRepository = serviceComponentRepository;
            _resolverRepository = resolverRepository;
            _unitOfWork = unitOfWork;
            _userIdentity = userIdentity;
        }

        public ServiceComponent GetById(int id)
        {
            ServiceComponent result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceComponentRepository.GetById(id);
            });
            return result;
        }

        public IEnumerable<ServiceComponent> All()
        {
            IEnumerable<ServiceComponent> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceComponentRepository.All().ToList();
            });

            return result;
        }

        public int Create(ServiceComponent entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceComponentRepository.Insert(entity);
                _unitOfWork.Save();
            });
            return entity.Id;
        }

        public List<ServiceOrganisationListItem> GetServiceOrganisationResolversByDesk(int serviceDeskId, string organisationType)
        {
            List<ServiceOrganisationListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceComponentRepository
                    .Query(x => x.ServiceFunction.ServiceDomain.ServiceDeskId == serviceDeskId && x.Resolver != null &&
                        x.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName == organisationType)
                    .Include(i => i.ParentServiceComponent)
                    .Include(i => i.ParentServiceComponent.ChildServiceComponents)
                    .Include(i => i.ServiceFunction)
                    .Include(i => i.ServiceFunction.ServiceDomain)
                    .Include(i => i.ServiceFunction.ServiceDomain.ServiceDesk)
                    .Include(i => i.ServiceFunction.ServiceDomain.ServiceDesk.Customer)
                    .Include(i => i.Resolver)
                    .Include(i => i.Resolver.OperationalProcessTypes)
                    .Include(i => i.Resolver.ServiceDeliveryOrganisationType)
                    .Include(i => i.Resolver.ServiceDeliveryUnitType)
                    .Include(i => i.Resolver.ResolverGroupType)
                    .Select(x => new ServiceOrganisationListItem
                    {
                        CustomerName = x.ServiceFunction.ServiceDomain.ServiceDesk.Customer.CustomerName,
                        ServiceDeskId = x.ServiceFunction.ServiceDomain.ServiceDeskId,
                        ServiceDeskName = x.ServiceFunction.ServiceDomain.ServiceDesk.DeskName,
                        ServiceDeliveryOrganisationTypeName = x.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName,
                        ServiceDeliveryUnitTypeName = x.Resolver.ServiceDeliveryUnitType != null ? x.Resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName : NoneSpecified,
                        ResolverGroupTypeName = x.Resolver.ResolverGroupType != null ? x.Resolver.ResolverGroupType.ResolverGroupTypeName : NoneSpecified,
                        ServiceComponent = x,
                        ServiceActivities = !string.IsNullOrEmpty(x.ServiceActivities) ? x.ServiceActivities : x.ComponentLevel == 2 && !string.IsNullOrEmpty(x.ParentServiceComponent.ServiceActivities) ? x.ParentServiceComponent.ServiceActivities : NoneSpecified,
                        Resolver = x.Resolver
                    })
                    .OrderBy(o => o.ServiceDeliveryOrganisationTypeName)
                    .ThenBy(o => o.ServiceDeliveryUnitTypeName)
                    .ThenBy(o => o.ResolverGroupTypeName).ToList();
            });

            return result;
        }

        public void Create(IEnumerable<ServiceComponent> entities)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceComponentRepository.AddRange(entities);
                _unitOfWork.Save();
            });
        }

        public void Update(ServiceComponent entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _serviceComponentRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Update(IEnumerable<ServiceComponent> entities)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                entities?.ForEach(f => _serviceComponentRepository.Update(f));
                _unitOfWork.Save();
            });
        }

        public void Delete(ServiceComponent entity)
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
                            _unitOfWork.ExecuteNonQueryStoredProcedure("spDeleteServiceComponent",
                                dbConnection,
                                transaction,
                                new SqlParameter("@ServiceComponentId", SqlDbType.Int) { Value = entity.Id });

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

        public IQueryable<ServiceComponent> GetByCustomer(int customerId)
        {
            IQueryable<ServiceComponent> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceComponentRepository
                    .Query(x => x.ServiceFunction.ServiceDomain.ServiceDesk.CustomerId == customerId)
                    .Include(i => i.ServiceFunction.ServiceDomain)
                    .Include(i => i.ChildServiceComponents)
                    .Include(i => i.Resolver)
                    .Include(i => i.Resolver.OperationalProcessTypes)
                    .Include(i => i.Resolver.ServiceDeliveryOrganisationType)
                    .Include(i => i.Resolver.ServiceDeliveryUnitType)
                    .Include(i => i.Resolver.ResolverGroupType);
            });

            return result;
        }

        public IQueryable<ServiceComponentListItem> GetByCustomerWithHierarchy(int customerId)
        {
            IQueryable<ServiceComponentListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceComponentRepository
                    .Query(x => x.ServiceFunction.ServiceDomain.ServiceDesk.CustomerId == customerId)
                    .Include(i => i.ChildServiceComponents)
                    .Include(i => i.Resolver)
                    .Include(i => i.Resolver.ServiceDeliveryOrganisationType)
                    .Include(i => i.Resolver.ServiceDeliveryUnitType)
                    .Include(i => i.Resolver.ResolverGroupType)
                    .AsNoTracking()
                    .Select(x => new ServiceComponentListItem
                    {
                        ServiceDeskName = x.ServiceFunction.ServiceDomain.ServiceDesk.DeskName,
                        ServiceDomainName = x.ServiceFunction.ServiceDomain.AlternativeName ??
                            x.ServiceFunction.ServiceDomain.DomainType.DomainName,
                        ServiceFunctionId = x.ServiceFunctionId,
                        ServiceFunctionName = x.ServiceFunction.AlternativeName ?? x.ServiceFunction.FunctionType.FunctionName,
                        ServiceComponent = x
                    });
            });

            return result;
        }

        public IQueryable<ResolverListItem> GetResolverByCustomerWithHierarchy(int customerId)
        {
            IQueryable<ResolverListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _serviceComponentRepository
                    .Query(x => x.ServiceFunction.ServiceDomain.ServiceDesk.CustomerId == customerId)
                    .Include(i => i.ServiceFunction)
                    .Include(i => i.ServiceFunction.ServiceDomain)
                    .Include(i => i.ServiceFunction.ServiceDomain.ServiceDesk)
                    .Include(i => i.Resolver)
                    .Include(i => i.Resolver.ServiceDeliveryOrganisationType)
                    .Include(i => i.Resolver.ServiceDeliveryUnitType)
                    .Include(i => i.Resolver.ResolverGroupType)
                    .Include(i => i.Resolver.ServiceDeliveryUnitNotes)
                    .AsNoTracking()
                    .Select(x => new ResolverListItem
                    {
                        Id = x.Resolver.Id,
                        ServiceDeskId = x.ServiceFunction.ServiceDomain.ServiceDeskId,
                        ServiceDeskName = x.ServiceFunction.ServiceDomain.ServiceDesk.DeskName,
                        ServiceDomainName =
                            x.ServiceFunction.ServiceDomain.AlternativeName ??
                            x.ServiceFunction.ServiceDomain.DomainType.DomainName,
                        ServiceFunctionName = x.ServiceFunction.AlternativeName ?? x.ServiceFunction.FunctionType.FunctionName,
                        ServiceComponentId = x.Id,
                        ServiceComponentName = x.ComponentName,
                        ServiceDeliveryOrganisationTypeName = x.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName,
                        ServiceDeliveryUnitTypeName = x.Resolver.ServiceDeliveryUnitType != null ? x.Resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName : null,
                        ResolverGroupName = x.Resolver.ResolverGroupType != null ? x.Resolver.ResolverGroupType.ResolverGroupTypeName : null,
                        ServiceDeliveryOrganisationNotes = x.Resolver.ServiceDeliveryOrganisationNotes,
                        ServiceDeliveryUnitNotes = x.Resolver.ServiceDeliveryUnitNotes
                    });
            });

            return result;
        }

        public void MoveResolver(int customerId, int sourceServiceComponentId, int destinationServiceComponentId)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                var sourceServiceComponent = GetByCustomer(customerId)
                    .SingleOrDefault(x => x.Id == sourceServiceComponentId);


                if (sourceServiceComponent == null)
                {
                    throw new SlmException($"The source Service Component with ID {sourceServiceComponentId} and customer ID {customerId} cannot be found.");
                }

                var destinationServiceComponent = GetByCustomer(customerId)
                    .SingleOrDefault(x => x.Id == destinationServiceComponentId);


                if (destinationServiceComponent == null)
                {
                    throw new SlmException($"The destination Service Component with ID {destinationServiceComponentId} and customer ID {customerId} cannot be found.");
                }

                if ((destinationServiceComponent.ChildServiceComponents != null && destinationServiceComponent.ChildServiceComponents.Any()) ||
                    destinationServiceComponent.Resolver != null)
                {
                    throw new SlmException($"The destination Service Component with ID {destinationServiceComponentId} already has dependencies.");
                }

                var userName = _userIdentity.Name;
                var now = DateTime.Now;

                var resolver = _resolverRepository.GetById(sourceServiceComponent.Resolver.Id);

                sourceServiceComponent.Resolver = null;
                sourceServiceComponent.UpdatedBy = userName;
                sourceServiceComponent.UpdatedDate = now;
                _serviceComponentRepository.Update(sourceServiceComponent);

                resolver.ServiceComponent = destinationServiceComponent;
                resolver.UpdatedBy = userName;
                resolver.UpdatedDate = now;
                _resolverRepository.Update(resolver);

                destinationServiceComponent.Resolver = resolver;
                destinationServiceComponent.UpdatedBy = userName;
                destinationServiceComponent.UpdatedDate = now;
                _serviceComponentRepository.Update(destinationServiceComponent);

                _unitOfWork.Save();
            });

        }

    }
}