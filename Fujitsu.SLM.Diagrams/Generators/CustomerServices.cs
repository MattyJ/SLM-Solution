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
    public class CustomerServices : IDiagramGenerator
    {
        private readonly IServiceDeskService _serviceDeskService;

        public CustomerServices(IServiceDeskService serviceDeskService)
        {
            if (serviceDeskService == null)
            {
                throw new ArgumentNullException(nameof(serviceDeskService));
            }

            _serviceDeskService = serviceDeskService;
        }

        public List<ChartDataListItem> Generate(int serviceDeskId, bool svcDomains = false, bool svcFunctions = false,
            bool svcComponents = false, bool resolvers = false, bool svcActivities = false, bool opProcs = false, string[] domainsSelected = null)
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
                serviceDeskChart.CreateServiceDeskWithInputs(serviceDesk);

                // Resolvers
                if (serviceDesk.Resolvers != null)
                {
                    var serviceDeskResolvers = serviceDesk.Resolvers.ToList();

                    if (serviceDeskResolvers.Any())
                    {
                        // Customer Owned Resolver Groups
                        var customerServices = serviceDeskResolvers.Where(x => x.ServiceDeliveryOrganisationType.Id == 2 &&
                                            x.ServiceDeliveryUnitType != null).ToList();
                        if (customerServices.Any())
                        {
                            var customerOwned = new ChartDataListItem
                            {
                                Id = 0,
                                CenteredTitle = serviceDesk.Customer.CustomerName + " Owned Resolver Groups",
                                Title = string.Empty,
                                Type = DecompositionType.ServiceDeliveryOrganisation.ToString(),
                                Units = new List<ChartDataListItem>(),
                            };

                            var distinctSdus =
                                customerServices.Select(x => x.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName)
                                    .Distinct();

                            foreach (var sduName in distinctSdus)
                            {
                                var sdu = new ChartDataListItem
                                {
                                    Id = 0,
                                    CenteredTitle = sduName,
                                    Title = string.Empty,
                                    Type = DecompositionType.ServiceDeliveryUnit.ToString(),
                                    Units = new List<ChartDataListItem>(),
                                };

                                var servicesNotes = string.Join("\r\n", customerServices.Where(
                                    x => x.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName == sduName)
                                    .Select(x => x.ServiceDeliveryUnitNotes)
                                    .ToArray());

                                if (!string.IsNullOrEmpty(servicesNotes))
                                {
                                    var services = new ChartDataListItem
                                    {
                                        Id = 0,
                                        Title = servicesNotes,
                                        CenteredTitle = string.Empty,
                                        Type = DecompositionType.CustomerServices.ToString(),
                                        Units = new List<ChartDataListItem>(),
                                    };

                                    sdu.Units.Add(services);
                                }
                                customerOwned.Units.Add(sdu);
                            }

                            serviceDeskChart.Units.Add(customerOwned);
                        }

                        // Customer Third Party Resolver Groups
                        var customerThirdPartyServices =
                            serviceDeskResolvers.Where(x => x.ServiceDeliveryOrganisationType.Id == 3 &&
                                            x.ServiceDeliveryUnitType != null).ToList();
                        if (customerThirdPartyServices.Any())
                        {
                            var customerThirdParty = new ChartDataListItem
                            {
                                Id = 0,
                                CenteredTitle = serviceDesk.Customer.CustomerName + " 3rd Party Resolver Groups",
                                Title = string.Empty,
                                Type = DecompositionType.ServiceDeliveryOrganisation.ToString(),
                                Units = new List<ChartDataListItem>(),
                            };

                            var distinctSdus =
                                customerThirdPartyServices.Select(
                                    x => x.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName)
                                    .Distinct();

                            foreach (var sduName in distinctSdus)
                            {
                                var sdu = new ChartDataListItem
                                {
                                    Id = 0,
                                    CenteredTitle = sduName,
                                    Title = string.Empty,
                                    Type = DecompositionType.ServiceDeliveryUnit.ToString(),
                                    Units = new List<ChartDataListItem>(),
                                };

                                var servicesNotes = string.Join("\r\n",
                                    customerThirdPartyServices.Where(
                                        x => x.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName == sduName)
                                        .Select(x => x.ServiceDeliveryUnitNotes)
                                        .ToArray());

                                if (!string.IsNullOrEmpty(servicesNotes))
                                {
                                    var services = new ChartDataListItem
                                    {
                                        Id = 0,
                                        Title = string.IsNullOrEmpty(servicesNotes) ? string.Empty : servicesNotes,
                                        CenteredTitle = string.Empty,
                                        Type = DecompositionType.CustomerServices.ToString(),
                                        Units = new List<ChartDataListItem>(),
                                    };

                                    sdu.Units.Add(services);
                                }

                                customerThirdParty.Units.Add(sdu);
                            }

                            serviceDeskChart.Units.Add(customerThirdParty);
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
