using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Diagrams.Entities;
using Fujitsu.SLM.Diagrams.Extensions;
using Fujitsu.SLM.Diagrams.Interfaces;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Diagrams.Generators
{
    public class CustomerThirdPartyServiceOrganisation : IDiagramGenerator
    {
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IServiceComponentService _serviceComponentService;
        private readonly string _serviceOrganisationDiagramtype;

        public CustomerThirdPartyServiceOrganisation(IServiceDeskService serviceDeskService, IServiceComponentService serviceComponentService)
        {
            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }

            if (serviceComponentService == null)
            {
                throw new ArgumentNullException(nameof(serviceComponentService));
            }

            _serviceDeskService = serviceDeskService;
            _serviceComponentService = serviceComponentService;
            _serviceOrganisationDiagramtype = ServiceDeliveryOrganisationNames.CustomerThirdParty;
        }

        public List<ChartDataListItem> Generate(int serviceDeskId, bool svcDomains = false, bool svcFunctions = false,
            bool svcComponents = false, bool resolvers = false, bool svcActivities = false, bool opProcs = false, string[] domainsSelected = null)
        {
            if (serviceDeskId == 0)
            {
                throw new ArgumentNullException("serviceDeskId");
            }

            var diagram = new List<ChartDataListItem>();

            var serviceDeskChart = new ChartDataListItem();

            var serviceDesk = _serviceDeskService.GetById(serviceDeskId);

            if (serviceDesk != null)
            {
                serviceDeskChart.CreateServiceDeskWithInputs(serviceDesk);

                var serviceDomains = serviceDesk.ServiceDomains.ToList();
                if (serviceDomains.Any())
                {
                    var serviceOrganisationListItems = _serviceComponentService.GetServiceOrganisationResolversByDesk(serviceDeskId, _serviceOrganisationDiagramtype);
                    if (serviceOrganisationListItems.Any())
                    {
                        if (resolvers)
                        {
                            // Resolvers and/or Service Components and/or Service Activities
                            serviceOrganisationListItems.ProcessResolvers(svcComponents, svcActivities,
                                _serviceOrganisationDiagramtype, serviceDesk.Customer.CustomerName, serviceDeskChart);
                        }
                        else if (svcComponents)
                        {
                            // Service Components and/or Service Activities
                            serviceOrganisationListItems.ProcessResolverServiceComponents(svcActivities,
                                _serviceOrganisationDiagramtype, serviceDeskChart);
                        }
                        else if (svcActivities)
                        {
                            // Service Activities only
                            serviceOrganisationListItems.ProcessResolverServiceActivities(false, true,
                                _serviceOrganisationDiagramtype, serviceDeskChart);
                        }
                    }
                }

                if (!serviceDeskChart.Units.Any())
                {
                    // Add Empty Unit
                    var chartDataListItem = new ChartDataListItem
                    {
                        Id = 0,
                        Title = string.Empty,
                        CenteredTitle = string.Empty,
                        Type = DecompositionType.EmptyForLayout.ToString(),
                        Units = new List<ChartDataListItem>(),
                    };

                    serviceDeskChart.Units.Add(chartDataListItem);
                }

                diagram.Add(serviceDeskChart);
            }

            return diagram;
        }
    }
}
