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
    public class ServiceDeskStructure : IDiagramGenerator
    {
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IResolverService _resolverService;

        public ServiceDeskStructure(IServiceDeskService serviceDeskService, IResolverService resolverService)
        {
            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }

            if (resolverService == null)
            {
                throw new ArgumentNullException(nameof(resolverService));
            }

            _serviceDeskService = serviceDeskService;
            _resolverService = resolverService;
        }

        public List<ChartDataListItem> Generate(int serviceDeskId,
            bool svcDomains = false,
            bool svcFunctions = false,
            bool svcComponents = false,
            bool resolvers = false,
            bool svcActivities = false,
            bool opProcs = false,
            string[] domainsSelected = null)
        {
            if (serviceDeskId == 0)
            {
                throw new ArgumentNullException(nameof(serviceDeskId));
            }

            var diagram = new List<ChartDataListItem>();

            var serviceDeskChart = new ChartDataListItem();

            var serviceDesk = _serviceDeskService.GetById(serviceDeskId);

            if (serviceDesk != null)
            {
                var customerName = serviceDesk.Customer.CustomerName;

                serviceDeskChart.CreateServiceDeskWithInputs(serviceDesk);

                var serviceDomains = serviceDesk.ServiceDomains.ToList();

                if (domainsSelected != null)
                {
                    if (!domainsSelected.Contains("0"))
                    {
                        // Only render the selected Service Domains
                        var selectedDomains = domainsSelected.ToList();
                        serviceDomains = serviceDomains.Where(x => selectedDomains.Contains(x.Id.ToString())).ToList();
                    }
                }


                if (serviceDomains.Any())
                {
                    var dotMatrix = _resolverService.GetDotMatrix(serviceDesk.CustomerId, true);

                    if (svcDomains || domainsSelected != null && domainsSelected.Any())
                    {
                        serviceDeskChart.ProcessServiceDomains(svcFunctions, svcComponents, resolvers, svcActivities, opProcs, customerName, serviceDomains, dotMatrix);

                    }
                    else if (svcFunctions)
                    {
                        foreach (var deskDomain in serviceDomains)
                        {
                            serviceDeskChart.ProcessServiceFunctions(svcComponents, resolvers, svcActivities, opProcs, customerName, deskDomain, dotMatrix);
                        }
                    }
                    else if (svcComponents)
                    {
                        foreach (var domainFunction in serviceDomains.Where(deskDomain => deskDomain.ServiceFunctions != null).SelectMany(deskDomain => deskDomain.ServiceFunctions))
                        {
                            serviceDeskChart.ProcessServiceComponents(resolvers, svcActivities, opProcs, customerName, domainFunction, dotMatrix);
                        }
                    }
                    else if (resolvers)
                    {
                        foreach (var component in from deskDomain in serviceDomains
                                                  where deskDomain.ServiceFunctions != null
                                                  from domainFunction in deskDomain.ServiceFunctions
                                                  where domainFunction.ServiceComponents != null
                                                  from component in domainFunction.ServiceComponents.Where(x => x.ComponentLevel == 1)
                                                  select component)
                        {
                            serviceDeskChart.ProcessResolvers(svcActivities, opProcs, customerName, component, dotMatrix);
                        }
                    }
                    else if (svcActivities)
                    {
                        foreach (var component in from deskDomain in serviceDomains
                                                  where deskDomain.ServiceFunctions != null
                                                  from domainFunction in deskDomain.ServiceFunctions
                                                  where domainFunction.ServiceComponents != null
                                                  from component in domainFunction.ServiceComponents.Where(x => x.ComponentLevel == 1)
                                                  select component)
                        {
                            serviceDeskChart.ProcessServiceActivities(opProcs, component, dotMatrix);
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
