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
    public class FujitsuDomains : IDiagramGenerator
    {
        private readonly IServiceDeskService _serviceDeskService;

        public FujitsuDomains(IServiceDeskService serviceDeskService)
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
                var customerName = serviceDesk.Customer.CustomerName;

                serviceDeskChart.CreateServiceDeskWithInputs(serviceDesk);

                var serviceDeliveryOrganisation = new ChartDataListItem
                {
                    Id = 0,
                    CenteredTitle = "Fujitsu",
                    Title = string.Empty,
                    Type = DecompositionType.ServiceDeliveryOrganisation.ToString(),
                    Units = new List<ChartDataListItem>(),
                };

                var serviceDomains = serviceDesk.ServiceDomains.ToList();

                if (serviceDomains.Any())
                {
                    serviceDeliveryOrganisation.ProcessServiceDomains(false, false, false, false, false, customerName, serviceDomains, null);
                }

                serviceDeskChart.Units.Add(serviceDeliveryOrganisation);

                diagram.Add(serviceDeskChart);
            }

            return diagram;
        }


    }
}
