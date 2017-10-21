using Fujitsu.Exceptions.Framework;
using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Core.Interfaces;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using Fujitsu.SLM.Services.Helpers;
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
    public class ResolverService : IResolverService
    {
        private readonly IRepository<Resolver> _resolverRepository;
        private readonly IRepository<OperationalProcessType> _operationalProcessTypeRepository;
        private readonly IRepository<OperationalProcessTypeRefData> _operationalProcessTypeRefDataRepository;
        private readonly IRepository<ServiceComponent> _serviceComponentRepository;
        private readonly IRepository<ServiceDesk> _serviceDeskRepository;
        private readonly IUserIdentity _userIdentity;
        private readonly IUnitOfWork _unitOfWork;
        private const string Id = "ServiceComponentId";

        public ResolverService(IRepository<Resolver> resolverRepository,
            IRepository<OperationalProcessType> operationalProcessTypeRepository,
            IRepository<OperationalProcessTypeRefData> operationalProcessTypeRefDataRepository,
            IRepository<ServiceComponent> serviceComponentRepository,
            IRepository<ServiceDesk> serviceDeskRepository,
            IUserIdentity userIdentity,
            IUnitOfWork unitOfWork)
        {
            if (resolverRepository == null)
            {
                throw new ArgumentNullException(nameof(resolverRepository));
            }

            if (operationalProcessTypeRepository == null)
            {
                throw new ArgumentNullException(nameof(operationalProcessTypeRepository));
            }

            if (operationalProcessTypeRefDataRepository == null)
            {
                throw new ArgumentNullException(nameof(operationalProcessTypeRefDataRepository));
            }

            if (serviceComponentRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentRepository));
            }

            if (serviceDeskRepository == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskRepository));
            }

            if (userIdentity == null)
            {
                throw new ArgumentNullException(nameof(userIdentity));
            }

            if (unitOfWork == null)
            {
                throw new ArgumentNullException(nameof(unitOfWork));
            }

            _resolverRepository = resolverRepository;
            _operationalProcessTypeRepository = operationalProcessTypeRepository;
            _operationalProcessTypeRefDataRepository = operationalProcessTypeRefDataRepository;
            _serviceComponentRepository = serviceComponentRepository;
            _serviceDeskRepository = serviceDeskRepository;
            _userIdentity = userIdentity;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<Resolver> All()
        {
            IEnumerable<Resolver> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _resolverRepository
                    .All()
                    .ToList();
            });

            return result;
        }

        public IQueryable<Resolver> GetByCustomer(int customerId)
        {
            IQueryable<Resolver> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _resolverRepository
                    .Query(x => x.ServiceDesk.CustomerId == customerId)
                    .Include(i => i.OperationalProcessTypes);
            });

            return result;
        }

        public IQueryable<ResolverListItem> GetListByCustomer(int customerId)
        {
            IQueryable<ResolverListItem> result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _resolverRepository
                    .Query(x => x.ServiceDesk.CustomerId == customerId)
                    .Include(i => i.OperationalProcessTypes)
                    .Select(x => new ResolverListItem
                    {
                        Id = x.Id,
                        ServiceDeskId = x.ServiceDeskId,
                        ServiceDeskName = x.ServiceDesk.DeskName,
                        ServiceDeliveryOrganisationTypeName = x.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName,
                        ServiceDeliveryUnitTypeName = x.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName,
                        ResolverGroupName = x.ResolverGroupType.ResolverGroupTypeName,
                        ServiceDeliveryOrganisationNotes = x.ServiceDeliveryOrganisationNotes,
                        ServiceDeliveryUnitNotes = x.ServiceDeliveryUnitNotes
                    });
            });

            return result;
        }

        public List<List<DotMatrixListItem>> GetDotMatrix(int customerId, bool standard)
        {
            return GetDotMatrix(customerId, standard, null);
        }

        public List<List<DotMatrixListItem>> GetDotMatrix(int customerId, bool standard, int? serviceDeskId)
        {
            var result = new List<List<DotMatrixListItem>>();

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                // Get the Op Process reference data.
                var opRefs = standard ? OperationalProcessHelper.GetStandardAndSelectedForCustomer(_operationalProcessTypeRefDataRepository,
                        _operationalProcessTypeRepository,
                        customerId) : OperationalProcessHelper.GetAllAndNotVisibleForCustomer(_operationalProcessTypeRefDataRepository,
                        _operationalProcessTypeRepository,
                        customerId);

                // Get all the Resolvers and associated Op Processes for this customer. Need a left join here on Op Process Type just in case none are selected.
                var resolverOpProcs = _resolverRepository
                    .Query(w => w.ServiceDesk.CustomerId == customerId &&
                        (!serviceDeskId.HasValue || w.ServiceDesk.Id == serviceDeskId.Value))
                    .Where(w => w.ResolverGroupType != null)
                    .GroupJoin(_operationalProcessTypeRepository.Query(w => true),
                        p => p.Id,
                        c => c.Resolver.Id,
                        (p, g) => g
                            .Select(c => new
                            {
                                ResolverId = p.Id,
                                ResolverName = p.ResolverGroupType.ResolverGroupTypeName,
                                OpProcId = c.OperationalProcessTypeRefData.Id
                            })
                            .DefaultIfEmpty(new
                            {
                                ResolverId = p.Id,
                                ResolverName = p.ResolverGroupType.ResolverGroupTypeName,
                                OpProcId = 0
                            }))
                    .SelectMany(g => g)
                    .ToList();

                // Get the distinct list of Resolvers.
                var resolvers = resolverOpProcs
                    .GroupBy(g => g.ResolverId)
                    .Select(s => s.First())
                    .ToList();

                // Now build the dictionary such that we end up with a list of dictionaries. The dictionary
                // will be keyed by the column name and have the appropriate value.
                foreach (var resGrp in resolvers.OrderBy(o => o.ResolverName))
                {
                    var componentName =
                        _serviceComponentRepository.Query(c => c.Resolver.Id == resGrp.ResolverId)
                            .AsNoTracking()
                            .Select(s => s.ComponentName)
                            .FirstOrDefault();

                    if (!string.IsNullOrEmpty(componentName))
                    {
                        var d = new List<DotMatrixListItem>
                        {
                            new DotMatrixListItem
                            {
                                Name = DotMatrixNames.ResolverId,
                                Value = resGrp.ResolverId
                            },
                            new DotMatrixListItem
                            {
                                Name = DotMatrixNames.ResolverName,
                                Value = resGrp.ResolverName,
                            },
                            new DotMatrixListItem
                            {
                                Name = DotMatrixNames.ComponentName,
                                Value = componentName,
                            },
                        };

                        foreach (var opRef in opRefs)
                        {
                            var has = resolverOpProcs
                                .Any(x => x.OpProcId == opRef.Id &&
                                          x.ResolverId == resGrp.ResolverId);
                            d.Add(new DotMatrixListItem
                            {
                                Name = string.Concat(DotMatrixNames.OpIdPrefix, opRef.Id),
                                DisplayName = opRef.OperationalProcessTypeName,
                                Value = has
                            });
                        }

                        result.Add(d);
                    }
                }

            });

            return result;
        }

        public Resolver GetById(int id)
        {
            Resolver result = null;

            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                result = _resolverRepository.GetById(id);
            });
            return result;
        }

        public int Create(Resolver entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverRepository.Insert(entity);
                _unitOfWork.Save();
            });
            return entity.Id;
        }

        public void Create(Resolver entity, bool save)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverRepository.Insert(entity);
                if (save) _unitOfWork.Save();
            });
        }

        public void Update(Resolver entity)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverRepository.Update(entity);
                _unitOfWork.Save();
            });
        }

        public void Update(Resolver entity, bool save)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverRepository.Update(entity);
                if (save) _unitOfWork.Save();
            });
        }

        public void Update(IEnumerable<Resolver> entities)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                if (entities != null)
                {
                    entities.ForEach(f => _resolverRepository.Update(f, Id));
                }
                _unitOfWork.Save();
            });
        }



        public void Delete(Resolver entity)
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
                            _unitOfWork.ExecuteNonQueryStoredProcedure("spDeleteResolver",
                                dbConnection,
                                transaction,
                                new SqlParameter("@ResolverId", SqlDbType.Int) { Value = entity.Id });

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

        public void Delete(Resolver entity, bool save)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                _resolverRepository.Delete(entity);
                if (save) _unitOfWork.Save();
            });
        }

        public void Move(int resolverId, int destinationDeskId)
        {
            RetryableOperation.Invoke(ExceptionPolicies.General, () =>
            {
                var resolver = _resolverRepository.GetById(resolverId);
                var oldDesk = _serviceDeskRepository.GetById(resolver.ServiceDeskId);
                var newDesk = _serviceDeskRepository.GetById(destinationDeskId);
                var now = DateTime.Now;

                resolver.ServiceDeskId = destinationDeskId;
                resolver.UpdatedDate = now;
                resolver.UpdatedBy = _userIdentity.Name;

                oldDesk.UpdatedDate = now;
                oldDesk.UpdatedBy = _userIdentity.Name; ;
                newDesk.UpdatedDate = now;
                newDesk.UpdatedBy = _userIdentity.Name;

                _resolverRepository.Update(resolver);
                _serviceDeskRepository.Update(oldDesk);
                _serviceDeskRepository.Update(newDesk);
                _unitOfWork.Save();
            });
        }
    }
}