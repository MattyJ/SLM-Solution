using Fujitsu.SLM.Constants;
using Fujitsu.SLM.Diagrams.Entities;
using Fujitsu.SLM.Enumerations;
using Fujitsu.SLM.Extensions;
using Fujitsu.SLM.Model;
using Fujitsu.SLM.Services.Entities;
using System.Collections.Generic;
using System.Linq;
using Diagram = Fujitsu.SLM.Constants.Diagram;

namespace Fujitsu.SLM.Diagrams.Extensions
{
    public static class ChartDataListItemExtensions
    {

        private const string NoneSpecified = "None Specified";
        private const string ToBeConfirmed = "To Be Confirmed";

        public static void CreateServiceDeskWithInputs(this ChartDataListItem serviceDeskChart, ServiceDesk serviceDesk)
        {
            if (serviceDesk.DeskInputTypes != null && serviceDesk.DeskInputTypes.Any())
            {
                // Add Service Desk Inputs
                foreach (var deskInput in serviceDesk.DeskInputTypes.OrderBy(o => o.InputTypeRefData.InputTypeNumber))
                {
                    var deskInputName = new ChartDataListItem
                    {
                        Id = deskInput.Id,
                        CenteredTitle = deskInput.InputTypeRefData.InputTypeName,
                        Title = string.Empty,
                        Type = DecompositionType.InputTypeName.ToString(),
                        Units = new List<ChartDataListItem>()
                    };

                    var deskInputNumber = new ChartDataListItem
                    {
                        Id = deskInput.Id,
                        CenteredTitle = deskInput.InputTypeRefData.InputTypeNumber.ToString(),
                        Title = string.Empty,
                        Type = DecompositionType.InputTypeNumber.ToString(),
                        Units = new List<ChartDataListItem>()
                    };

                    var emptyForLayout = new ChartDataListItem
                    {
                        Id = 0,
                        CenteredTitle = string.Empty,
                        Type = DecompositionType.EmptyForLayout.ToString()
                    };

                    serviceDeskChart.Inputs.Add(deskInputName);
                    deskInputName.Units.Add(deskInputNumber);
                    deskInputNumber.Units.Add(emptyForLayout);
                }

                serviceDeskChart.Width = serviceDeskChart.Inputs.Count * Diagram.ShapeWidth;
            }

            serviceDeskChart.CenteredTitle = serviceDesk.DeskName;
            serviceDeskChart.Type = DecompositionType.Desk.ToString();
        }

        public static void ProcessServiceDomains(this ChartDataListItem chartData, bool svcFunctions,
            bool svcComponents,
            bool resolvers,
            bool svcActivities,
            bool opProcs,
            string customerName,
            IEnumerable<ServiceDomain> serviceDomains,
            List<List<DotMatrixListItem>> dotMatrix)
        {
            if (serviceDomains == null) return;

            foreach (var deskDomain in serviceDomains.OrderBy(x => x.DiagramOrder).ThenBy(x => x.DomainType.DomainName))
            {
                var domainChartData = new ChartDataListItem
                {
                    Id = deskDomain.Id,
                    Title = deskDomain.AlternativeName ?? deskDomain.DomainType.DomainName,
                    Type = DecompositionType.Domain.ToString(),
                    CenteredTitle = string.Empty,
                    Units = new List<ChartDataListItem>()
                };

                if (svcFunctions)
                {
                    domainChartData.ProcessServiceFunctions(svcComponents, resolvers, svcActivities, opProcs, customerName, deskDomain, dotMatrix);
                }
                else if (svcComponents)
                {
                    if (deskDomain.ServiceFunctions != null)
                    {
                        foreach (var domainFunction in deskDomain.ServiceFunctions)
                        {
                            domainChartData.ProcessServiceComponents(resolvers, svcActivities, opProcs, customerName, domainFunction, dotMatrix);
                        }
                    }
                }
                else if (resolvers)
                {
                    if (deskDomain.ServiceFunctions != null)
                    {
                        foreach (var component in deskDomain.ServiceFunctions.Where(domainFunction => domainFunction.ServiceComponents != null).SelectMany(domainFunction => domainFunction.ServiceComponents.Where(x => x.ComponentLevel == 1)))
                        {
                            domainChartData.ProcessResolvers(svcActivities, opProcs, customerName, component, dotMatrix);
                        }
                    }
                }
                else if (svcActivities)
                {
                    if (deskDomain.ServiceFunctions != null)
                    {
                        foreach (var component in deskDomain.ServiceFunctions.Where(domainFunction => domainFunction.ServiceComponents != null).SelectMany(domainFunction => domainFunction.ServiceComponents.Where(x => x.ComponentLevel == 1)))
                        {
                            domainChartData.ProcessServiceActivities(opProcs, component, dotMatrix);
                        }
                    }
                }

                chartData.Units.Add(domainChartData);
            }
        }

        public static void ProcessServiceFunctions(this ChartDataListItem chartData,
            bool svcComponents,
            bool resolverGroups,
            bool svcActivities,
            bool opProcs,
            string customerName,
            ServiceDomain deskDomain,
            List<List<DotMatrixListItem>> dotMatrix)
        {
            if (deskDomain.ServiceFunctions == null) return;

            foreach (var domainFunction in deskDomain.ServiceFunctions.OrderBy(x => x.DiagramOrder).ThenBy(x => x.FunctionType.FunctionName))
            {
                var function = new ChartDataListItem
                {
                    Id = domainFunction.Id,
                    Title = domainFunction.AlternativeName ?? domainFunction.FunctionType.FunctionName,
                    Type = DecompositionType.Function.ToString(),
                    CenteredTitle = string.Empty,
                    Units = new List<ChartDataListItem>(),
                };

                if (svcComponents)
                {
                    function.ProcessServiceComponents(resolverGroups, svcActivities, opProcs, customerName, domainFunction, dotMatrix);
                }
                else if (resolverGroups)
                {
                    if (domainFunction.ServiceComponents != null)
                    {
                        foreach (var component in domainFunction.ServiceComponents.Where(x => x.ComponentLevel == 1))
                        {
                            function.ProcessResolvers(svcActivities, opProcs, customerName, component, dotMatrix);
                        }
                    }
                }
                else if (svcActivities)
                {
                    if (domainFunction.ServiceComponents != null)
                    {
                        foreach (var component in domainFunction.ServiceComponents.Where(x => x.ComponentLevel == 1))
                        {
                            function.ProcessServiceActivities(opProcs, component, dotMatrix);
                        }
                    }
                }

                chartData.Units.Add(function);
            }
        }

        public static void ProcessServiceComponents(this ChartDataListItem chartData,
            bool resolvers,
            bool svcActivities,
            bool opProcs,
            string customerName,
            ServiceFunction domainFunction,
            List<List<DotMatrixListItem>> dotMatrix)
        {
            if (domainFunction.ServiceComponents == null) return;

            foreach (var parentComponent in domainFunction.ServiceComponents.Where(x => x.ComponentLevel == 1).OrderBy(x => x.DiagramOrder).ThenBy(x => x.ComponentName).ToList())
            {
                var parentComponentListItem = new ChartDataListItem
                {
                    Id = parentComponent.Id,
                    Title = parentComponent.ComponentName,
                    Type = DecompositionType.Component.ToString(),
                    CenteredTitle = string.Empty,
                    Units = new List<ChartDataListItem>()
                };

                if (parentComponent.ChildServiceComponents != null && parentComponent.ChildServiceComponents.Count > 0)
                {
                    foreach (var childComponent in parentComponent.ChildServiceComponents.OrderBy(x => x.DiagramOrder).ThenBy(x => x.ComponentName))
                    {
                        // Add Child Component
                        var childComponentListItem =
                            new ChartDataListItem
                            {
                                Id = childComponent.Id,
                                Title = childComponent.ComponentName,
                                Type = DecompositionType.Component.ToString(),
                                CenteredTitle = string.Empty,
                                Units = new List<ChartDataListItem>()
                            };

                        if (resolvers)
                        {
                            childComponentListItem.ProcessResolvers(svcActivities, opProcs, customerName, childComponent, dotMatrix);
                        }
                        else if (svcActivities)
                        {
                            childComponentListItem.ProcessServiceActivities(opProcs, childComponent, dotMatrix);
                        }

                        parentComponentListItem.Units.Add(childComponentListItem);
                    }
                }
                else if (resolvers || svcActivities)
                {
                    // Create A Dummy Child Component
                    var childComponentListItem = new ChartDataListItem
                    {
                        Id = 0,
                        Title = string.Empty,
                        CenteredTitle = string.Empty,
                        Type = DecompositionType.LineForDummyChildComponent.ToString(),
                        Units = new List<ChartDataListItem>()
                    };

                    if (resolvers)
                    {
                        childComponentListItem.ProcessResolvers(svcActivities, opProcs, customerName, parentComponent, dotMatrix);
                    }
                    else
                    {
                        childComponentListItem.ProcessServiceActivities(opProcs, parentComponent, dotMatrix);
                    }

                    if (childComponentListItem.Units.Any())
                    {
                        parentComponentListItem.Units.Add(childComponentListItem);
                    }

                }

                chartData.Units.Add(parentComponentListItem);
            }
        }

        public static void ProcessResolvers(this ChartDataListItem chartData,
            bool svcActivities,
            bool opProcs,
            string customerName,
            ServiceComponent component,
            List<List<DotMatrixListItem>> dotMatrix)
        {
            if (component.ChildServiceComponents != null && component.ChildServiceComponents.Any())
            {
                foreach (var childComponent in component.ChildServiceComponents)
                {
                    ProcessComponentResolver(svcActivities, opProcs, customerName, childComponent, dotMatrix, chartData);
                }
            }
            else
            {
                ProcessComponentResolver(svcActivities, opProcs, customerName, component, dotMatrix, chartData);
            }
        }

        public static void ProcessServiceActivities(this ChartDataListItem chartData,
            bool opProcs,
            ServiceComponent component,
            List<List<DotMatrixListItem>> dotMatrix)
        {

            if (component.ChildServiceComponents != null &&
                component.ChildServiceComponents.Any())
            {
                foreach (var childComponent in component.ChildServiceComponents)
                {
                    // Process Child Component Service Activity
                    ProcessComponentServiceActivity(opProcs, childComponent, dotMatrix, chartData);
                }
            }
            else
            {
                // Process Parent Component Service Activity
                ProcessComponentServiceActivity(opProcs, component, dotMatrix, chartData);
            }

        }

        private static void ProcessComponentResolver(bool svcActivities,
            bool opProcs,
            string customerName,
            ServiceComponent component,
            List<List<DotMatrixListItem>> dotMatrix,
            ChartDataListItem chartData)
        {
            var resolver = new ChartDataListItem
            {
                Id = component.Id,
                Title = ToBeConfirmed,
                TitleTwo = string.Empty,
                TitleThree = string.Empty,
                Type = DecompositionType.Resolver.ToString(),
                CenteredTitle = string.Empty,
                Units = new List<ChartDataListItem>()
            };

            if (component.Resolver != null)
            {
                var serviceDeliveryOrganisation =
                    component.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName.SafeEquals(ServiceDeliveryOrganisationNames.Fujitsu)
                        ? component.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName
                        : component.Resolver.ServiceDeliveryOrganisationType.ServiceDeliveryOrganisationTypeName.SafeEquals(ServiceDeliveryOrganisationNames.Customer) ? customerName : $"{customerName} Third Party";

                // Add Resolver Group
                resolver = new ChartDataListItem
                {
                    Id = component.Id,
                    Title = serviceDeliveryOrganisation,
                    TitleTwo = !string.IsNullOrEmpty(component.Resolver.ServiceDeliveryUnitType?.ServiceDeliveryUnitTypeName) ? component.Resolver.ServiceDeliveryUnitType.ServiceDeliveryUnitTypeName : ToBeConfirmed,
                    TitleThree = !string.IsNullOrEmpty(component.Resolver.ResolverGroupType?.ResolverGroupTypeName) ? component.Resolver.ResolverGroupType.ResolverGroupTypeName : ToBeConfirmed,
                    Type = DecompositionType.Resolver.ToString(),
                    CenteredTitle = string.Empty,
                    Units = new List<ChartDataListItem>()

                };
            }

            if (svcActivities)
            {
                resolver.ProcessServiceActivities(opProcs, component, dotMatrix);
            }
            else if (opProcs)
            {
                ProcessOpProcs(component, dotMatrix, resolver);
            }

            chartData.Units.Add(resolver);
        }

        private static void ProcessComponentServiceActivity(
            bool opProcs,
            ServiceComponent component,
            List<List<DotMatrixListItem>> dotMatrix,
            ChartDataListItem chartData)
        {
            var activities = new ChartDataListItem
            {
                Id = component.Resolver?.Id ?? component.Id,
                Title = NoneSpecified,
                Type = DecompositionType.Activity.ToString(),
                CenteredTitle = string.Empty,
                Units = new List<ChartDataListItem>()
            };

            // Add Service Activity
            activities.Id = component.Id;
            if (!string.IsNullOrEmpty(component.ServiceActivities))
            {
                activities.Title = component.ServiceActivities;
            }
            else if (component.ComponentLevel == (int)ServiceComponentLevel.Level2 && !string.IsNullOrEmpty(component.ParentServiceComponent.ServiceActivities))
            {
                activities.Title = component.ParentServiceComponent.ServiceActivities;
            }

            chartData.Units.Add(activities);

            if (opProcs)
            {
                ProcessOpProcs(component, dotMatrix, activities);
            }
        }

        private static void ProcessOpProcs(ServiceComponent component,
            List<List<DotMatrixListItem>> dotMatrix,
            ChartDataListItem chartData)
        {
            // Add Service Activity
            if (component.Resolver != null)
            {
                // Get the data for this resolver group.
                var componentDotMatrix = dotMatrix
                    .FirstOrDefault(x => x.Any(y => y.Name == DotMatrixNames.ResolverId &&
                                                               (int)y.Value == component.Resolver.Id));

                var last = chartData;
                if (componentDotMatrix != null)
                {
                    var opProcs = componentDotMatrix
                        .Where(x => x.Name.StartsWith(DotMatrixNames.OpIdPrefix))
                        .ToList();

                    foreach (var opProc in opProcs)
                    {
                        var opProcChartData =
                            new ChartDataListItem
                            {
                                Title = opProc.DisplayName,
                                Type = (bool)opProc.Value
                                    ? DecompositionTypeNames.ResolverGroupOperationalProcessSelected
                                    : DecompositionTypeNames.ResolverGroupOperationalProcess,
                                Units = new List<ChartDataListItem>(),
                                CenteredTitle = string.Empty,
                            };
                        last.Units.Add(opProcChartData);
                        last = opProcChartData;
                    }
                }
            }
        }
    }
}