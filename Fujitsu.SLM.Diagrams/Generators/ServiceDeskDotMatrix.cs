using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Diagrams.Entities;
using Fujitsu.SLM.Diagrams.Interfaces;
using Fujitsu.SLM.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fujitsu.SLM.Diagrams.Generators
{
    public class ServiceDeskDotMatrix : IDiagramGenerator
    {
        private readonly IServiceDeskService _serviceDeskService;
        private readonly IResolverService _resolverService;

        public ServiceDeskDotMatrix(IServiceDeskService serviceDeskService, IResolverService resolverService)
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

        public List<ChartDataListItem> Generate(int serviceDeskId, bool svcDomains = false, bool svcFunctions = false,
           bool svcComponents = false, bool resolvers = false, bool svcActivities = false, bool opProcs = false, string[] domainsSelected = null)
        {
            if (serviceDeskId == 0)
            {
                throw new ArgumentNullException(nameof(serviceDeskId));
            }

            var diagram = new List<ChartDataListItem>();

            var serviceDesk = _serviceDeskService.GetById(serviceDeskId);

            if (serviceDesk != null)
            {
                var dotMatrixData = _resolverService.GetDotMatrix(serviceDesk.CustomerId, true, serviceDeskId);

                if (dotMatrixData == null || dotMatrixData.Count <= 0) throw new ApplicationException($"No Process Dot Matrix diagram data could be found for Service Desk Id [{serviceDeskId}]");

                // Construct the first row which holds the operational process names. The first block will be empty as it is above the resolver group namnes.
                var opProcsRow = new ChartDataListItem
                {
                    Title = string.Empty,
                    CenteredTitle = string.Empty,
                    Type = DecompositionTypeNames.EmptyForLayout,
                    Units = new List<ChartDataListItem>()
                };

                var opProcesses = dotMatrixData[0]
                    .Where(x => x.Name.StartsWith(DotMatrixNames.OpIdPrefix))
                    .ToList();

                foreach (var opProc in opProcesses)
                {
                    var deepestOpProcChartData = GetDeepest(opProcsRow);
                    var opProcChartData = new ChartDataListItem
                    {
                        Title = string.Empty,
                        CenteredTitle = opProc.DisplayName,
                        Type = DecompositionTypeNames.OperationalProcess,
                        Units = new List<ChartDataListItem>()
                    };

                    deepestOpProcChartData.Units.Add(opProcChartData);
                }

                diagram.Add(opProcsRow);

                // Construct the data
                foreach (var dotMatrix in dotMatrixData)
                {
                    // Get the resolver group item.
                    var resolverNameItem = dotMatrix.Single(x => x.Name == DotMatrixNames.ResolverName).Value.ToString();
                    var serviceComponentNameItem = dotMatrix.Single(x => x.Name == DotMatrixNames.ComponentName).Value.ToString();

                    var resolverNameChartData = new ChartDataListItem
                    {
                        Title = $"{resolverNameItem}",
                        TitleTwo = serviceComponentNameItem.Length < 100 ? $"[{serviceComponentNameItem}]" : $"[{serviceComponentNameItem.Substring(0, 94)} ...]",
                        CenteredTitle = string.Empty,
                        Type = DecompositionTypeNames.Resolver,
                        Units = new List<ChartDataListItem>()
                    };

                    diagram.Add(resolverNameChartData);

                    // Loop round the values for the Op Procs.
                    var opProcResolvers = dotMatrix
                        .Where(x => x.Name.StartsWith(DotMatrixNames.OpIdPrefix))
                        .ToList();
                    foreach (var opProcResolver in opProcResolvers)
                    {
                        var resolverGroupOpProcChartData = (bool)opProcResolver.Value
                            ? new ChartDataListItem
                            {
                                Title = string.Empty,
                                CenteredTitle = string.Empty,
                                Type = DecompositionTypeNames.ResolverGroupOperationalProcessSelected,
                                Units = new List<ChartDataListItem>()
                            }
                            : new ChartDataListItem
                            {
                                Title = string.Empty,
                                CenteredTitle = string.Empty,
                                Type = DecompositionTypeNames.ResolverGroupOperationalProcess,
                                Units = new List<ChartDataListItem>()
                            };
                        var deepestGroupOpProcChartData = GetDeepest(resolverNameChartData);
                        deepestGroupOpProcChartData.Units.Add(resolverGroupOpProcChartData);
                    }
                }

                diagram.Reverse();
            }

            return diagram;
        }

        private static ChartDataListItem GetDeepest(ChartDataListItem chartDataListItem)
        {
            while (true)
            {
                if (chartDataListItem.Units == null || chartDataListItem.Units.Count == 0)
                {
                    return chartDataListItem;
                }

                chartDataListItem = chartDataListItem.Units.First();
            }
        }
    }
}
