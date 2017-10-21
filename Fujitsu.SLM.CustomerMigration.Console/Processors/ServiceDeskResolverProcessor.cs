using Fujitsu.SLM.CustomerMigration.Console.Interfaces;
using Fujitsu.SLM.Data;
using Fujitsu.SLM.Data.Interfaces;
using Fujitsu.SLM.Model;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.CustomerMigration.Console.Processors
{
    public class ServiceDeskResolverProcessor : IProcessor
    {
        private readonly ServiceDesk _serviceDesk;
        private readonly IEnumerable<Resolver> _resolvers;
        private readonly ISLMDataContext _dataContext;
        private readonly IUnitOfWork _unitOfWork;

        public ServiceDeskResolverProcessor(ServiceDesk serviceDesk,
            IEnumerable<Resolver> resolvers,
            ISLMDataContext dataContext,
            IUnitOfWork unitOfWork)
        {
            _serviceDesk = serviceDesk;
            _resolvers = resolvers;
            _dataContext = dataContext;
            _unitOfWork = unitOfWork;
        }

        public void Execute()
        {
            foreach (var resolver in _resolvers)
            {
                int sqlResult;
                using (var sourceDbContext = new SLMDataContext("Name=SLMSourceDataContext"))
                {
                    sqlResult = sourceDbContext.Database.SqlQuery<int>($"SELECT Count(*) FROM RESOLVER WHERE Id = {resolver.Id} AND ServiceComponent_Id IS NULL").FirstOrDefault();
                }

                if (sqlResult > 0)
                {
                    if (resolver.ServiceDeliveryUnitType != null)
                    {
                        if (!_dataContext.ServiceDeliveryUnitTypeRefData.Any(x => x.ServiceDeliveryUnitTypeName == resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName))
                        {
                            var serviceDeliveryUnitType = new ServiceDeliveryUnitTypeRefData
                            {
                                ServiceDeliveryUnitTypeName = resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName,
                                SortOrder = resolver.ServiceDeliveryUnitType.SortOrder,
                                Visible = resolver.ServiceDeliveryUnitType.Visible
                            };

                            _dataContext.ServiceDeliveryUnitTypeRefData.Add(serviceDeliveryUnitType);
                            _unitOfWork.Save();
                        }
                    }

                    if (resolver.ResolverGroupType != null)
                    {
                        if (!_dataContext.ResolverGroupTypeRefData.Any(x => x.ResolverGroupTypeName == resolver.ResolverGroupType.ResolverGroupTypeName))
                        {
                            var resolverGroupType = new ResolverGroupTypeRefData
                            {
                                ResolverGroupTypeName = resolver.ResolverGroupType.ResolverGroupTypeName,
                                SortOrder = resolver.ResolverGroupType.SortOrder,
                                Visible = resolver.ResolverGroupType.Visible
                            };

                            _dataContext.ResolverGroupTypeRefData.Add(resolverGroupType);
                            _unitOfWork.Save();
                        }
                    }

                    var newResolver = new Resolver
                    {
                        ServiceDesk = _serviceDesk,
                        ServiceDeskId = _serviceDesk.Id,
                        ServiceDeliveryOrganisationType = _dataContext.ServiceDeliveryOrganisationTypeRefData.Single(x => x.ServiceDeliveryOrganisationTypeName == resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName),
                        ServiceDeliveryOrganisationNotes = resolver.ServiceDeliveryOrganisationNotes,
                        ServiceDeliveryUnitNotes = resolver.ServiceDeliveryUnitNotes,
                        ServiceDeliveryUnitType = resolver.ServiceDeliveryUnitType != null ? _dataContext.ServiceDeliveryUnitTypeRefData.Single(x => x.ServiceDeliveryUnitTypeName == resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName) : null,
                        ResolverGroupType = resolver.ResolverGroupType != null ? _dataContext.ResolverGroupTypeRefData.Single(x => x.ResolverGroupTypeName == resolver.ResolverGroupType.ResolverGroupTypeName) : null,
                        InsertedBy = resolver.InsertedBy,
                        InsertedDate = resolver.InsertedDate,
                        UpdatedBy = resolver.UpdatedBy,
                        UpdatedDate = resolver.UpdatedDate
                    };

                    _serviceDesk.Resolvers.Add(newResolver);
                    _unitOfWork.Save();
                }
            }
        }
    }
}